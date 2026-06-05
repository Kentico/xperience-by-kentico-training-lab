## Goal

Automate content migration from one content type to another in Xperience by Kentico, generating code that will handle complex relationships including parent-child relationships among the migrated content and references from other content types.

## Platform

This prompt was created for GitHub Copilot, but your agents should be able to help you quickly make any format adjustments for it to work with other agentic tools.

## Use

1. Copy the `input`, `instructions`, and `prompts` folders into your `.github` folder.
1. Copy the `migration-input-template.md` file, rename it to something relevant, and fill it in with 
information about the migration you want to perform.
1. Install the [documentation MCP server](https://docs.kentico.com/x/mcp_server_xp).
1. Call the **migration-create-implementation** prompt, directing your agent to your input file and telling it which project to create the migration code in. For example:
    ```
    /migration-create-implementation 
    
    Create an implementation for the migration described in product-consolidation-input.md in the TrainingGuides.Web project
    ```
