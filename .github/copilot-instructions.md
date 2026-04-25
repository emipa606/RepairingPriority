# Copilot Instructions for Repairing Priority Mod

This document provides guidance to GitHub Copilot users on developing and maintaining the Repairing Priority mod for RimWorld using C#. It includes an overview, features, coding patterns, XML integration details, Harmony patching, and suggestions for using Copilot effectively.

## Mod Overview and Purpose

**Repairing Priority** is a RimWorld mod that allows players to prioritize repair duties for their pawns beyond the default home area restrictions. It is inspired by the Cleaning Priority mod by ChippedChaps and was conceptualized by Protok. This mod provides a strategic advantage by enabling players to specify which structures, such as base defenses, should be repaired first after challenging events like sieges, enhancing gameplay customization and efficiency.

## Key Features and Systems

- **Custom Repair Areas**: Define specific areas that should be prioritized for repairs, which can lie outside the traditional home zones.

- **User Interface Customization**: Offers a dialog window (`Dialog_RepairingPriority`) for setting and adjusting repair priorities.

- **Hover Tooltips**: Implement tooltips to provide additional information when hovering over prioritized areas.

- **Area Management**: Includes methods for adding, removing, and prioritizing repair areas within `RepairManager_MapComponent`.

- **Worker Systems**: Specialized `WorkGiver` classes (`WorkGiver_FixBrokenDownBuildingPrioritized` and `WorkGiver_RepairPrioritized`) manage job assignments based on new prioritization logic.

## Coding Patterns and Conventions

- **File Naming Conventions**: Follow a clear and descriptive naming convention such as `Area_`, `BreakdownManager_`, and `RepairingPriority` prefixes to reflect the functionality or component they manage.
  
- **Class and Method Access Modifiers**: Use `internal` access modifiers for classes and methods to encapsulate the mod's functionality and maintain integrity within the assembly.

- **Method Naming**: Methods are often prefixed with action-oriented verbs, maintaining a consistent pattern. For example, `AddAreaRange`, `RemoveAreaRange`, `EnsureHasAtLeastOneArea`.

- **Interfaces and Extensions**: Implement interfaces such as `ICellBoolGiver` where necessary to extend core functionalities (as seen in `RepairManager_MapComponent`).

## XML Integration

- Although this mod focuses mainly on C# code, XML can be used for defining additional game data or settings if needed. XML patches can target game data without recompilation; consider adding XML files for future extendibility, especially to define new areas or repair types.

## Harmony Patching

- **Harmony Library**: Utilize the Harmony library for runtime patching of RimWorld's core methods to modify or extend functionality without altering the original game code. This is essential for injecting custom behavior, such as altering the repair job logic.

- **Patch Applications**: Implement patches where critical game methods related to repair and area management need to be overridden or extended.

## Suggestions for Copilot

1. **Contextual Code Generation**: When generating code with Copilot, provide context such as the purpose of the new method or class with clear comments or function descriptions.

2. **Harmony Patching Assistance**: Let Copilot assist in writing Harmony patches by starting with clear intent comments, describing what original method behavior needs to change.

3. **UI Enhancements**: Use Copilot to suggest improvements or additions to user interface elements like dialogs and tooltips by describing intended use in comments.

4. **Refactor Suggestions**: Look for Copilot's recommendations on refactoring existing methods for efficiency or readability improvements.

5. **Error Handling**: In critical sections, prompt Copilot to suggest robust error handling routines, ensuring stability during runtime.

By adhering to these guidelines and suggestions, you can effectively utilize GitHub Copilot to support the ongoing development and enhancement of the Repairing Priority mod for RimWorld.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.
