High level description of changes done during refactoring.
======================
#### 1. Downgraded .NET to 4.5
 Required .NET version was downgraded from 4.6.1 to 4.5 (this might have resulted in different behaviour of Task object but appropriate changes were implemented - see below for details).

#### 2. Replaced VSTO harness with ADX harness

ADX is a set of libraries and deployment helpers, which help to develop and deploy version-neutral, bitness-neutral .NET assembly as COM plugin to Microsoft Office. This eliminates VSTO dependency. ADX components are commercial and royalty-free. Source code of all ADX components is available for licensing.

#### 3. Add-in assemblies have strong name
A COM plugin assembly must have strong name, as well as all referenced assemblies. For this reason, all assemblies obtained through NuGet must be signed with strong name if they are not. `/3rdparty/makesn.cmd` is used to generate key file, and sign 3rd party assemblies obtained through NuGet with that key. The signing is done by disassembling and reassembling, and it requires Windows SDK for disassembly tool (`ildasm.exe`).

#### 4. WiX setup project and `InstallerCA` project
It is OK to continue using InstallShield setup project, however I created WiX setup project since I am not InstallShield specialist. The WiX setup project uses new project `InstallerCA` to install ExcelDNA assemblies. Source code for `InstallerCA` was taken from ExcelDNA GitHub repo with minimal changes.

`Quandl.Excel.AddinSetup.wixproj` is WiX setup project, mostly generated automatically by ADX. ADX tools can generate InstallShield setup project too. ADX uses `adxregistrator.exe` to execute custom actions associated with installing and uninstalling plugin (plus rollback phase); this custom action can be copied to existing InstallShield project -- generate a new setup project with InstallShield for reference.

#### 5. `Quandl.Excel.Addin` project 
`Quandl.Excel.Addin` project is a class library built with ADX components. The `UI` folder contains WPF forms with minimal changes. 
  
`MainLogic` class works as a class library to replace `Globals.ThisAddIn` construct with `MainLogic.Instance`.
However, the task panes are no longer floating since floating task panes do not work well on dual-monitor systems, and they are not designed to be aligned with parent window. The task panes are now docked to the right, except for the Function Wizard which is now not a TaskPane but independent form. A set of helper functions to facilitate this behavior was implemented. The second argument to `UpdateTaskPane` method takes `false` to use a docked task pane and `true` to use a separate form. See below:

```
logic.TaskPaneUpdater.UpdateTaskPane<UI.AboutControlHost> (adxTaskPaneAbout,false);
logic.TaskPaneUpdater.UpdateTaskPane<UI.UpdateControlHost> (adxTaskPaneUpdater,false); 
logic.TaskPaneUpdater.UpdateTaskPane<UI.SettingsControlHost> (adxTaskPaneSettings,false);   
logic.TaskPaneUpdater.UpdateTaskPane<UI.WizardGuideControlHost> (adxTaskPaneBuilder,true); 
```

`FunctionUpdater.cs` was moved there from `Shared` module, it was not developed from scratch. GitHub does not detect this.
However it was changed to correctly release COM references.

Original project used `Properties.Settings` class to store localizable resources such as strings, which is not the preferred design. These settings have been moved to `Properties.Resources`.

Original project used to store reference to COM object (`ActiveCells`) on its instance member, which makes impossible to correctly release the COM object. This has been changed through several utility functions - COM objects are not cached for long-term, and they are released correctly once we do not need them any longer. This change was applied throughout the solution.

XAML files were slighly modified to use `Source="msappx:///Resources` instead of `Source="/Quandl.Excel.Addin;component/Resources` per design guidelines.

A few localizable strings hardcoded into `.cs` files were moved to `Properties.Resources`.

Global event handlers have been removed since it is very easy to add delegates to them which are never released (and that was the case with the code). These handlers were not needed for the addin to work correctly.

`settings.xaml.cs` was modified slightly to avoid referencing WCF object directly from a method executed asynchronously.
Also, validation procedure was fixed to give error message if API key entered into box is incorrect.

`TaskPaneWpfControlHost.cs` is a new helper class. Task panes which must be displayed as such are based on this class, but each task pane must be implemented with its own COM-visible object (see inherited classes in the same `.cs` file).

`FormulaInserted.xaml.cs` is not a new file, but it was decoupled from Excel COM object model.

`Update.xaml.cs` was modified to allow lazy initialization of updater, preventing race condition on start-up.

#### 6. `Quandl.Excel.UDF.Functions` and `Quandl.Shared.Modules` projects

`Quandl.Excel.UDF.Functions` was modified to use Fody weaver to produce single assembly by combining all referenced assemblies into a single DLL, and then combine this assembly with binary xll file which comes along with ExcelDNA. This two-step action happens automatically at compile time and produces two packed XLL files (32bit and 64bit) which are later deployed without any other dependent DLLs. ExcelDNA was updated to allow for this behavior.

All references to StatusBar were replaced with calls to a shared implementation of `IHostService` interface. Such implementations are different for the Add-in project and UDF function project, but in either case they are using a well-known instance of Excel object instead of querying ROT through `GetObject` function. 

`Shared.Helpers.HttpHelper.EnableTlsSupport` method was implemented to reuse the same code in both applications.

`SheetHelper.cs` file was refactored to release all referenced COM objects.

Mutex is no longer used to block foreground thread.

All calls to `Task.Wait()` method have been removed. They are redundant since we do not care whether task was executed on its own thread or synchronously. At the same time, I have seen `Task.Wait()` calls resulting in tasks never activated by task scheduler (possibly depends on framework implementation). A better design would be to restructure the flow by using explicitly one thread to do background calls to the server (or two threads), while using foreground thread from the Addin (not from the function) to update Excel. Such redesign would eliminate excessive concurrent tasks and provide much more robust operation of the solution. At the same time, most of the code working with Excel objects can be reused.

`DummyHostService` class is used by class factory to prevent `NullReferenceException` if the host did not initialize it properly (e.g. from a new unit test project or if initialization failed).

`ExcelExecutionHelper` is used to handle retries on Excel API calls in centralized manner.

`Update` class was updated to allow for lazy initialization.

`Utilities` class was slightly updated for better handling of exceptional situations.