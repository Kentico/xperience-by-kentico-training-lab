## Goal

Refactor the Training Guides repository to use the new ContentRetriever API instead of the old Content item query API (where applicable).

## Repository refactored/experimented on

The instruction an supporting files have been used on the [finished branch of the Training guides repo](https://github.com/Kentico/xperience-by-kentico-training-guides/tree/finished).

## Tools used

- GitHub Copilot in Agent mode
- LLM: Claude Sonnet 4
- Code examples for before and after refactoring are sourced from the following code bases:
    - Dancing Goat sample site
    - [Xperience by Kentico Kickstart](https://github.com/Kentico/xperience-by-kentico-kickstart/)
    - [Training Guides - _main_ (a.k.a. _starter_ branch)](https://github.com/Kentico/xperience-by-kentico-training-guides/tree/main)

## Notes

Post AI refactor there were some minor adjustments necessary for specific scenarios, although the AI was able to cover most scenarios. The instruction file contains a note for the AI to mark places in code explicitly that need a thorough review.
