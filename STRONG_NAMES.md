# Strong names to all assemblies

Restore NuGet packages and run script "3rdParty\makesn.cmd" to create assemblies with strong names.

All 3rd party dependency assemblies must have strong names. Repeat the following steps for octokit.dll, Syroot.Windows.IO.KnownFolders.dll, Markdown.Xaml.dll

1. Generate a KeyFile: sn -k keyPair.snk
2. Obtain the MSIL for the provided assembly: ildasm providedAssembly.dll /out:providedAssembly.il
3. Rename/move the original assembly: ren providedAssembly.dll providedAssembly.dll.orig
4. Create a new assembly from the MSIL output and your assembly KeyFile: ilasm providedAssembly.il /dll /key=keyPair.snk

