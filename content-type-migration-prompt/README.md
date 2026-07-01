## Goal

Automate content migration from one content type to another in Xperience by Kentico, generating code that will handle complex relationships including parent-child relationships among the migrated content and references from other content types.

## Advantages

While the [management MCP server](https://docs.kentico.com/documentation/developers-and-admins/api/management-api) allows your agents to directly create content items, it is ideal for small, development-centered scenarios.

In scenarios where there are large numbers of items to remodel, having your agent transfer each item one by one is costly in tokens or credits.

We recommend using an agent to create code that utilizes Xperience APIs to remodel the content items for better efficiency.

## Platform

This prompt was created for GitHub Copilot, but your agent should be able to help you quickly make any format adjustments for it to work with other agentic tools.

## Example

TODO

## Usage

1. Create a local copy of your project in a safe development environment.
1. Copy the `input`, `instructions`, and `prompts` folders into your `.github` folder.
1. Copy the `migration-input-template.md` file, rename it to something relevant, and fill it in with 
information about the migration you want to perform.
    - Optionally, install the [management MCP server](https://docs.kentico.com/documentation/developers-and-admins/api/management-api) to help you quickly access data, such as class form definitions, to fill in the template.
1. Install the [documentation MCP server](https://docs.kentico.com/x/mcp_server_xp).
1. Call the **migration-create-implementation** prompt, directing your agent to your input file and telling it which project to create the migration code in. For example:
    ```
    /migration-create-implementation 
    
    Create an implementation for the migration described in product-consolidation-input.md in the TrainingGuides.Web project
    ```
1. Back up your database to quickly restore the pre-migration state in your development environment. 
1. Access the generated controller's URL to initiate the migration.
1. Check the resulting log for any errors, and inspect the migrated items.
1. Iterate with your agent to address any issues in the migration script.
1. After it is ready, run the migration on production data during a maintenance window.
    - Make sure the trigger for the migration is not publicly accessible. If you must run it in a public-facing environment, change the controller to require a secret key, or implement a different way to trigger the migration code.
1. Delete the old items after migrating
    - (Consider expanding the script to include this functionality)
1. Make sure to remove the one-time code before deploying to production.