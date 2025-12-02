import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import {
  CallToolRequestSchema,
  ListToolsRequestSchema
} from "@modelcontextprotocol/sdk/types.js";
import fs from "fs/promises";
import path from "path";
import OpenAI from "openai";

// Initialize OpenAI client for LM Studio
const openai = new OpenAI({
  baseURL: "http://localhost:1234/v1",
  apiKey: "lm-studio", // LM Studio doesn't require a real key
});

interface ResourceEntry {
  name: string;
  value: string;
  comment?: string;
  xmlSpace?: string;
  schemaDef?: string;
}

// Parse .resx XML file
function parseResourceFile(content: string): { entries: ResourceEntry[], header: string, footer: string } {
  const entries: ResourceEntry[] = [];
  
  // Extract header (everything before first <data> tag)
  const firstDataMatch = content.match(/<data\s/);
  const headerEnd = firstDataMatch ? content.indexOf(firstDataMatch[0]) : content.length;
  const header = content.substring(0, headerEnd);
  
  // Extract footer (everything after last </data> tag)
  const lastDataEndMatch = content.lastIndexOf('</data>');
  const footer = lastDataEndMatch !== -1 ? content.substring(lastDataEndMatch + 7) : '\n</root>';
  
  // Parse data entries
  const dataRegex = /<data\s+name="([^"]+)"(?:\s+xml:space="([^"]+)")?\s*>([\s\S]*?)<\/data>/g;
  let match;
  
  while ((match = dataRegex.exec(content)) !== null) {
    const name = match[1];
    const xmlSpace = match[2];
    const innerContent = match[3];
    
    // Extract comment if exists
    const commentMatch = innerContent.match(/<comment>([\s\S]*?)<\/comment>/);
    const comment = commentMatch ? commentMatch[1].trim() : undefined;
    
    // Extract value
    const valueMatch = innerContent.match(/<value>([\s\S]*?)<\/value>/);
    const value = valueMatch ? valueMatch[1].trim() : '';
    
    entries.push({
      name,
      value,
      comment,
      xmlSpace,
    });
  }
  
  return { entries, header, footer };
}

// Chunk entries into batches
function chunkArray<T>(array: T[], chunkSize: number): T[][] {
  const chunks: T[][] = [];
  for (let i = 0; i < array.length; i += chunkSize) {
    chunks.push(array.slice(i, i + chunkSize));
  }
  return chunks;
}

// Translate a batch of resources
async function translateBatch(
  entries: ResourceEntry[],
  targetLanguage: string,
  batchNumber: number,
  totalBatches: number
): Promise<ResourceEntry[]> {
  const resourceText = entries
    .map(e => `${e.name}=${e.value}`)
    .join('\n');

  const prompt = `You are translating .NET resource strings from English to ${targetLanguage}.

IMPORTANT RULES:
1. Only translate the VALUES after the = sign
2. Keep the KEYS (resource names) unchanged
3. Preserve placeholders like {0}, {1}, {{0}}, etc. exactly as they appear
4. Preserve special characters, line breaks (\\n, \\r\\n), and formatting
5. Return ONLY the translated resources in the same key=value format
6. Do not add explanations or additional text

Resources to translate (batch ${batchNumber}/${totalBatches}):

${resourceText}

Translated resources:`;

  try {
    const completion = await openai.chat.completions.create({
      model: "local-model", // LM Studio uses whatever model is loaded
      messages: [
        {
          role: "system",
          content: `You are a professional translator specializing in software localization. Translate resource values to ${targetLanguage} while preserving all technical formatting. Ensure the output contains valid xml code. CRITICAL: remove any non-valid xml and html tags, namely <lt;/a>, <lt;/bgt, <brgt;, <bgt;, "&lt;b&gt;, &lt;b&gt;, multiple <br><br> tags, or <br > </ br> tags.`
        },
        {
          role: "user",
          content: prompt
        }
      ],
      temperature: 0.3,
      max_tokens: 4000,
    });

    const translatedText = completion.choices[0].message.content?.trim() || '';
    
    // Parse the translated output
    const translatedLines = translatedText.split('\n');
    const translatedEntries: ResourceEntry[] = [];

    for (const line of translatedLines) {
      const trimmed = line.trim();
      if (trimmed && trimmed.includes('=')) {
        const equalIndex = trimmed.indexOf('=');
        const name = trimmed.substring(0, equalIndex).trim();
        const value = trimmed.substring(equalIndex + 1).trim();
        
        // Find original entry to preserve metadata
        const originalEntry = entries.find(e => e.name === name);
        translatedEntries.push({
          name,
          value,
          comment: originalEntry?.comment,
          xmlSpace: originalEntry?.xmlSpace,
        });
      }
    }

    return translatedEntries;
  } catch (error) {
    throw new Error(`Translation failed for batch ${batchNumber}: ${error}`);
  }
}

const schemaDef = `<xsd:schema id="root" xmlns="" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:msdata="urn:schemas-microsoft-com:xml-msdata">
    <xsd:import namespace="http://www.w3.org/XML/1998/namespace" />
    <xsd:element name="root" msdata:IsDataSet="true">
      <xsd:complexType>
        <xsd:choice maxOccurs="unbounded">
          <xsd:element name="metadata">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" />
              </xsd:sequence>
              <xsd:attribute name="name" use="required" type="xsd:string" />
              <xsd:attribute name="type" type="xsd:string" />
              <xsd:attribute name="mimetype" type="xsd:string" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="assembly">
            <xsd:complexType>
              <xsd:attribute name="alias" type="xsd:string" />
              <xsd:attribute name="name" type="xsd:string" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="data">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
                <xsd:element name="comment" type="xsd:string" minOccurs="0" msdata:Ordinal="2" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" msdata:Ordinal="1" />
              <xsd:attribute name="type" type="xsd:string" msdata:Ordinal="3" />
              <xsd:attribute name="mimetype" type="xsd:string" msdata:Ordinal="4" />
              <xsd:attribute ref="xml:space" />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name="resheader">
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name="value" type="xsd:string" minOccurs="0" msdata:Ordinal="1" />
              </xsd:sequence>
              <xsd:attribute name="name" type="xsd:string" use="required" />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name="resmimetype">
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name="version">
    <value>2.0</value>
  </resheader>
  <resheader name="reader">
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name="writer">
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
`;  

// Format entries as .resx XML
function formatResourceFile(entries: ResourceEntry[], header: string, footer: string): string {
  const dataElements = entries.map(entry => {
    const xmlSpaceAttr = entry.xmlSpace ? ` xml:space="${entry.xmlSpace}"` : '';
    const commentSection = entry.comment ? `\n    <comment>${entry.comment}</comment>` : '';
    
    return `  <data name="${entry.name}"${xmlSpaceAttr}>${commentSection}
    <value>${entry.value}</value>
  </data>`;
  });
  
  return header + schemaDef + dataElements.join('\n') + footer;
}


const server = new Server(
  {
    name: "ai-localization-translator",
    version: "1.0.0",
  },
  {
    capabilities: {
      tools: {},
    },
  }
);


server.setRequestHandler(ListToolsRequestSchema, async () => {
  return {
    tools: [
      {
        name: "translate_localization_file",
        description: "Translates a .NET .resx localization file to a target language using LM Studio. Processes the file in batches and outputs valid XML format.",
        inputSchema: {
          type: "object",
          properties: {
            sourceFile: {
              type: "string",
              description: "Path to the source localization file (e.g., .../data/localization.server.en-US.txt)",
            },
            targetLanguage: {
              type: "string",
              description: "Target language code (e.g., es-ES for Spanish, fr-FR for French)",
            },
            chunkSize: {
              type: "number",
              description: "Number of resources to process per batch (default: 100)",
              default: 100,
            },
          },
          required: ["sourceFile", "targetLanguage"],
        },
      },
    ],
  };
});

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  switch (request.params.name) {

    case "translate_localization_file": {
      const { sourceFile, targetLanguage, chunkSize = 100 } = request.params.arguments as {
        sourceFile: string;
        targetLanguage: string;
        chunkSize?: number;
      };

      try {
        // Read source file
        const sourcePath = path.resolve(sourceFile);
        const content = await fs.readFile(sourcePath, 'utf-8');
        
        // Parse resources
        const { entries, header, footer } = parseResourceFile(content);
        const totalEntries = entries.length;
        
        // Create chunks
        const chunks = chunkArray(entries, chunkSize);
        const totalBatches = chunks.length;
        
        const translatedEntries: ResourceEntry[] = [];
        const progressMessages: string[] = [];

        progressMessages.push(`Starting translation: ${totalEntries} resources in ${totalBatches} batches`);
        progressMessages.push(`Target language: ${targetLanguage}`);
        progressMessages.push(`Connecting to LM Studio at http://localhost:1234/v1`);
        progressMessages.push('---');

        // Process each batch
        for (let i = 0; i < chunks.length; i++) {
          const batchNumber = i + 1;
          progressMessages.push(`Processing batch ${batchNumber}/${totalBatches} (${chunks[i].length} resources)...`);
          
          const translated = await translateBatch(
            chunks[i],
            targetLanguage,
            batchNumber,
            totalBatches
          );
          
          translatedEntries.push(...translated);
          progressMessages.push(`✓ Batch ${batchNumber}/${totalBatches} complete`);
        }

        // Generate output filename
        const sourceDir = path.dirname(sourcePath);
        const sourceBase = path.basename(sourcePath);
        const outputFilename = sourceBase.replace(/\.en-US\.txt$/, `.${targetLanguage}.txt`);
        const outputPath = path.join(sourceDir, outputFilename);

        // Write translated file in XML format
        const outputContent = formatResourceFile(translatedEntries, header, footer);
        await fs.writeFile(outputPath, outputContent, 'utf-8');

        progressMessages.push('---');
        progressMessages.push(`✓ Translation complete!`);
        progressMessages.push(`Output file: ${outputPath}`);
        progressMessages.push(`Translated ${translatedEntries.length} resources in valid .resx XML format`);

        return {
          content: [
            {
              type: "text",
              text: progressMessages.join('\n'),
            },
          ],
        };
      } catch (error) {
        return {
          content: [
            {
              type: "text",
              text: `Error during translation: ${error instanceof Error ? error.message : String(error)}`,
            },
          ],
          isError: true,
        };
      }
    }

    default:
      throw new Error(`Unknown tool: ${request.params.name}`);
  }
});

async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("MCP Server running on stdio");
}

main().catch((error) => {
  console.error("Fatal   error:", error);
  process.exit(1);
});