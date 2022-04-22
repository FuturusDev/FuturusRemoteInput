# Unity Package Template

This template mostly follows the recommended layout from [Unity](https://docs.unity3d.com/Manual/cus-layout.html).

## Divergent

A documentation folder and other meta-data related files were ommitted by my personal preference.

Internal variables in the runtime assembly are visible to the editor assembly.

# Importing into a project

If your package has not been published to UPM (Unity Package Manager) a project's manifest may be manually edited to include it.

The manifest may be found at `your-project/Packages/manifest.json`

`"your-package-name": "your-package-github-repo-url"` may be added to dependencies of your project in the manifest.json. `your-package-github-repo-url` should in `.git`.

Unity will automatically download the latest version of the repo into your project as a package separate from your Assets folder.
