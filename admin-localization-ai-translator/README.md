# Admin Localization AI Translator

An MCP (Model Context Protocol) server that translates .NET `.resx` localization files using AI models through LM Studio.

## Purpose

This MCP server automates the translation of Xperience by Kentico localization resource files from English to other languages. It:

- Processes `.resx` XML localization files in batches.
- Preserves resource keys, placeholders, and formatting.
- Maintains XML structure and metadata (comments, xml:space attributes).
- Uses local AI models via [LM Studio](https://lmstudio.ai/) for translation.
  - Built with [OpenAI REST API](https://www.npmjs.com/package/openai), you can swap a local LLM for a proper hosted LLM service that uses standard OpenAI API.
- Outputs properly formatted XML files ready for use.

## Prerequisites

- **Node.js** (v18 or later)
- **LM Studio** installed and running locally
  - Download from [lmstudio.ai](https://lmstudio.ai)
  - Load a model suitable for translation (recommended: multilingual models like Mixtral, Qwen, or similar)
  - Start the local server (default: `http://localhost:1234`)

## Setup

### 1. Install Dependencies

```bash
npm install
```

### 2. Build the Server

```
npm run server:build
```

### 3. Configure LM Studio

   1. Launch LM Studio.
   2. Load a translation-capable model.
   3. Start the local server (Server tab → Start Server).
   4. Verify it's running at `http://localhost:1234`.
   5. Add to MCP Client.

### 4. Add this server to your MCP client configuration (e.g., VS Code, Claude Desktop, Cline):

For VS Code, follow [official documentation](https://code.visualstudio.com/docs/copilot/customization/mcp-servers), i.e., `https://code.visualstudio.com/docs/copilot/customization/mcp-servers`

Example `mcp.json` file:

```json
{
	"servers": {
		"xperience-ai-admin-localization": {
			"type": "stdio",
			"command": "node",
			"args": ["dist/server.js"],
			"cwd":"${workspaceFolder}/admin-localization-ai-translator",
		}
	},
	"inputs": []
}
```

For Claude Desktop (claude_desktop_config.json):

```json
{
  "mcpServers": {
    "ai-localization-translator": {
      "command": "node",
      "args": ["c:/dev/xperience-by-kentico-training-lab/admin-localization-ai-translator/dist/server.js"]
    }
  }
}
```
For Cline (VS Code settings):

```json
{
  "cline.mcpServers": {
    "ai-localization-translator": {
      "command": "node",
      "args": ["c:/dev/xperience-by-kentico-training-lab/admin-localization-ai-translator/dist/server.js"]
    }
  }
}
```

### 5. (optional) Start the server in dev mode

```
npm run server:dev
```

## Usage

Tool: `translate_localization_file`
Translates a source localization file to a target language.

### Parameters

| Parameter | Type | Required | Description | Default |
|-----------|------|----------|-------------|---------|
| sourceFile | string | Yes | Path to the source .resx file (e.g., localization.server.en-US.txt) | - |
| targetLanguage | string | Yes | Target language code (e.g., es-ES, fr-FR, de-DE) | - |
| chunkSize | number | No | Number of resources to translate per batch | 100 |

## Example Usage

Through an MCP client like Claude or Cline:

```
Translate the file data/localization.server.en-US.txt to Spanish (es-ES)
```
Or with a custom chunk size:
```
Translate data/localization.server.en-US.txt to French (fr-FR) using a chunk size of 50
```

## Source Data

### Adding Source Files

Place your source .resx localization files in the `data/` directory:

```
admin-localization-ai-translator/
  data/
    localization.server.en-US.txt    ← Your source file
    localization.admin.en-US.txt     ← Additional source files
```

File Format

Source files must be valid `.resx` XML format:

```
<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- Header content -->
  <data name="ResourceKey" xml:space="preserve">
    <comment>Optional comment</comment>
    <value>Resource value to translate</value>
  </data>
  <!-- More data entries -->
</root>
```

Recommendation: Change the file's type to `.txt`. (E.g., Rename the `.resx` to `.txt`).

Output Files

Translated files are automatically saved in the same directory with the target language code:

- Input: `localization.server.en-US.txt`
- Output: `localization.server.es-ES.txt` (for Spanish)

### Translation Process

1. Parsing: Extracts resource entries from the source .resx file (temporarily changed to `.txt`).
2. Batching: Splits resources into chunks for efficient processing.
3. Translation: Sends each batch to LM Studio with preservation rules.
4. Validation: Ensures keys, placeholders, and formatting are preserved.
5. Output: Generates a properly formatted XML file, but as a `.txt`.
6. Change the output file from `.txt` to `.resx`.

What Gets Preserved

- ✅ Resource keys (names)
- ✅ Placeholders ({0}, {1}, {{0}}, etc.)
- ✅ Special characters and escape sequences
- ✅ Line breaks (\n, \r\n)
- ✅ XML attributes (xml:space="preserve")
- ✅ Comments
- ✅ XML structure and formatting

## Troubleshooting

### LM Studio Connection Issues

If you see connection errors:

1. Verify LM Studio is running.
2. Check the server is on port `1234`.
3. Ensure a model is loaded.
   
### Poor Translation Quality

1. Try a different model in LM Studio.
2. Reduce chunkSize for more context-aware translations.
3. Use models specifically trained for multilingual tasks.

### Output File Issues

1. Verify the source file is valid XML.
2. Check file permissions in the data directory.
3. Ensure the source file follows `.resx` format conventions.

## Development

### Project Structure

```
admin-localization-ai-translator/
  src/
    server.ts           ← MCP server implementation
  data/                 ← Place source files here
  dist/                 ← Compiled JavaScript (after build)
  package.json          ← Dependencies and scripts
```

### Modifying the Server

1. Edit `src/server.ts`.
2. Rebuild: `npm run server:build`.
3. Restart your MCP client to pick up changes.

### Adjusting Translation Prompts

Edit the `translateBatch()` function in server.ts to customize translation instructions or temperature settings.

## License

Part of the Xperience by Kentico Training Lab repository.