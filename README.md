# About
This small C# program is able to load a given JSON file, parse it, and allow searching for nodes based off of certain selectors.
	class - The view class name, e.g. "StackView"
	classNames - CSS class names, e.g. ".container"
	identifier - The view identifier, e.g. "#videoMode"

The program also properly supports compound selectors (e.g. "View#identifier"), and selector chains (e.g. "StackView .container").


# How To Build
	1. Download the source either as a zip or through git.
	2. Open up the .csproj in Visual Studio. If it complains about versions, then create a new project and use the source files directly.
	3. Build!

# Final Remarks
While I believe this matches the requirements specifications given, if there was anything missing, or any bugs found, I will be supporting this for the foreseeable future.