# Contributing Setup

## Required Software

There are no software requirements to contribute to this repository.

## Development Workflow

1. Create a new branch with a meaningful name reflecting the purpose and topic of your contribution.
1. Add a new folder with a name meaningfully reflecting the gist of your AI refactoring/migration experiments.

   - Include a README file inside your new folder describing the folder's contents, following this structure:
      ```
      ## Goal

      What were you trying to achieve/what you achieved?

      ## Repository refactored/experimented on

      What code bases did you perform the refactoring/migration on?

      ## Tools used

      - What AI tool did you use (e.g., GitHUb Copilot, Coursor, ...)?
      - Which LLM (e.g., Claude Sonnet 4, GPT-5,...)?
      - If you included code samples, where are they source from?

      ## Notes

      Any additional notes and remarks

      ```
1. Commit changes, with a commit message preferably following the [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/#summary) convention.

1. Once ready, create a PR on GitHub. The PR will need to have all comments resolved before it will be merged.

   - The PR should have a helpful description of the scope of changes being contributed.
   - Include screenshots or video to reflect UX or UI updates
   - Indicate if new settings need to be applied when the changes are merged - locally or in other environments
