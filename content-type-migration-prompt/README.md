# Remodel content within Xperience by Kentico

While the [Kentico Migration Tool](https://github.com/Kentico/xperience-by-kentico-kentico-migration-tool) provides opportunity to remodel content while migrating from older versions of Kentico to *Xperience by Kentico (XbyK)*, it cannot help projects that started out on XbyK, or projects that initially required minimal migrations due to time or budget constraints.

This tool can help you generate one-time code that utilizes Xperience APIs to remodel your content by migrating data from one content type to another.

## Goal

- Generate code that migrates data from a source content type to a target content type in Xperience by Kentico. 
- Handle complex relationships, including parent-child relationships among the migrated content and references from other content types.

## Advantages of this approach

While the [management MCP server](https://docs.kentico.com/x/management_api_xp) allows your agent to directly create content items, it is only ideal for small, development-centered scenarios.

In scenarios with large numbers of items to remodel, having your agent transfer each item one by one is costly in tokens or credits.

For such scenarios, we recommend using an agent to generate code that handles the remodeling using Xperience APIs.

## Platform

This prompt was created for GitHub Copilot, but your agent should be able to help you quickly make any format adjustments for it to work with other agentic tools.

## Case study

We wanted to test the prompt on a relatively complex remodel that touched multiple types of relationships, so we used the following scenario to test this prompt:

In the [Training guides repository](https://github.com/Kentico/xperience-by-kentico-training-guides/tree/finished)'s `finished` branch, merge the **Cat food**, **Cat food variant**, **Dog collar**, and **Dog collar variant** content types into a single product content type, which represents both parent products and variants with a similar self-referential pattern to the **SKU** class in older Kentico versions.

We used GitHub Copilot with Claude Opus 4.6 to generate the code based on the input template in [the provided example](./example/input/product-consolidation-input.md).

After the initial run, the log contained errors about failing to create certain content item references. We quickly discovered this was a result of duplicate references from multiple language versions of the same item, and adjusted a conditional statement. After this change the code migrated all items without error.

We adjusted the prompt with a new instruction to avoid this error in future runs.

You can see the results in the [example output](./example/output/OneTimeCode/).

Feel free to run the code on [the contemporary commit of the branch](https://github.com/Kentico/xperience-by-kentico-training-guides/tree/29ae57516245a35b1440b3a8f85cb892fa83aa8d) to see the result!

## Usage

1. Stand up copy of your production project (database and codebase) in a safe development environment.
    - Keep a database backup on hand, so you can quickly restore the pre-migration database in your development environment. 
1. Add the target content type and [generate its code files](https://docs.kentico.com/x/5IbWCQ).
    - Optionally, the [management MCP server](https://docs.kentico.com/x/management_api_xp) enables your agentic coding assistant to add content types for you, so you don't need to click through the UI.
1. Copy the `input`, `instructions`, and `prompts` folders into your `.github` folder.
1. Copy the `migration-input-template.md` file, rename it to something relevant, and fill it in with 
information about the migration you want to perform.
    - Optionally, the [management MCP server](https://docs.kentico.com/x/management_api_xp) can help you quickly access data, such as class form definitions, to fill in the template.
1. Install the [documentation MCP server](https://docs.kentico.com/x/mcp_server_xp). (This is required by the prompt.)
1. Call the **migration-create-implementation** prompt, directing your agent to your input file and telling it which project to create the migration code in. For example:
    ```
    /migration-create-implementation 
    
    Create an implementation for the migration described in product-consolidation-input.md in the TrainingGuides.Web project
    ```
1. Access the generated controller's URL to initiate the migration, then check the resulting log for any errors and inspect the migrated items.
1. Iterate with your agent to address any shortcomings in the migration code, restoring the pre-migration database as needed.
1. After the code is ready, converting items without issue in the development environment, run the migration on production data during a maintenance window.
    - Make sure the trigger for the migration is not publicly accessible. If you must run it in a public-facing environment, change the controller to require a secret key, or implement a different way to trigger the migration code, such as a custom module page in the admin UI.
    - The safest way is to make new local copies of the production database and project, apply the remodeling migration, then redeploy both.
1. Delete items of the old content type after migrating. 
    - Optionally, have your agent expand the generated code to include deletion functionality.
1. Make sure to remove any one-time code related to the remodeling before deploying code files back to production.