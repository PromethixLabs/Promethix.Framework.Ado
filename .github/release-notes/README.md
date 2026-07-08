# Release Notes

RC and stable promotion workflows expect a checked-in Markdown file for the target version.

Default convention:

- `.github/release-notes/<version>.md`

Examples:

- `.github/release-notes/2.1.7-rc1.md`
- `.github/release-notes/2.1.7.md`

The RC and stable promotion workflows allow an override via the `release_notes_file` workflow input, but the default convention is recommended for normal use.

Suggested process:

1. Create the release notes file in a branch or directly on the approved release commit.
2. Review the Markdown as part of release approval.
3. Run the promotion workflow with `new_package_version` matching the filename.
4. Let the workflow publish the package, create or update the GitHub Release, and upload release assets using the same notes.

Recommended sections:

- `Summary`
- `Highlights`
- `Fixes and Improvements`
- `Compatibility`
- `Breaking Changes`
- `Upgrade Notes`
- `Known Issues`
- `Provenance`
