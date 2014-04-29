## Wide Ruled 2 Story Authoring System 

### Project Information
* Go to [my project page here](http://skorupski.org/wiki/wide_ruled/wide_ruled_v2) for more general project information and publications, screenshots, etc.

### General Directory Structure

* **`.\src_java\`** : This directory contains all the java source for the project, along with a sample generated ABL agent to test compile the java support code
* **`.\src_dotnet\WideRuled2\`** : This directory contains all of the Wide Ruled 2 interface and code generation source, along with build directories (described below) that contain all files required to generate ABL code, compile it, and launch the Java-based WideRuled2 backend. 
* **`.\src_dotnet\Xceed DataGrid 3.1\`** : Installation binary for the Visual Studio authoring/configuration interface for the Xceed Datagrids used in Wide Ruled 2. Without this, you must configure the DataGrids using the xaml source, within each <DataGridControl> node. The project is already configured with a purchased license key to use this tool at this specific version. 



###  .NET Build Directory Structures

Build Directories are located in: .\src_dotnet\WideRuled2\bin\Debug\ and .\src_dotnet\WideRuled2\bin\Release\ . Here are the subdirectories:

* **`.\Sample Story\`** : Story that is packaged with release of WR2
* **`.\abl\`** : Directory that contains all ABL compilation and code generation related files
* **`.\abl\ablcode\`** : Target directory where Wide Ruled 2 will place its Generated ABL agent, along with copied java files from the abl/static folder, in order to compile the ABL agent from Java source into Java class files
* **`.\abl\static\`** : Location of all files that are ONLY READ AND COPIED by Wide Ruled 2, in order to generate the ABL Agent code. This directory contains java support code source, along with precompiled Java WME class files that are required to generate the ABL java code with the ABL compiler
* **`.\abl\static\abl\`** : Extra, prewritten static ABL source code that is included in the generated agent (various daemons)
* **`.\abl\static\javacode\`** : Precompiled WMEs that are required alongside our ABL generated file in order for the ABL compiler to turn our .abl file into .java files
* **`.\abl\static\java\`** : Java source files for sensors, actions, and any support code, that needs to be compiled alongside our .java files that were created by the ABL compiler
* **`.\abl\jdk6\`** : A full copy of the Java JDK6 32 bit version 1.6.0_05. This is packaged with the project, so that installation does not require installing and configuring the JDK separately. 
* **`.\abl\abl.jar`** and **`.\abl\hoj.jar`** : ABL compiler JAR distribution and accompanying Higher-Order Java JAR library files


### Important Notes
* This project was built and compiled using **Microsoft Visual Studio 2008** - newer VS versions have not been tested (and likely require modifications and a newer version of the Xceed Data Grid and newer license key). 
* The Data Grids in Wide Ruled 2 use the Xceed WPF Datagrid control - Binary Installer for the Version used in WR2 is contained
* The Visual Authoring/Configuration of the Datagrids requires installation of the Xceed software, which is included inside the **`.\src_dotnet\`** directory
* If you don't want to install the Xceed DataGrid configuration tool, you can manually modify the configuration of the DataGrids directly in the XAML source for each window, within the `<DataGridControl>` node attributes and the subnodes within that parent node.
* The Story Generation process first begins in the `buttonGenerate_Click()` function in `WindowMain.xaml.cs`, which is triggered by clicking the Generate button. This function executes all the error checking logic, then starts the code generation and compilation phases. 
* Code generation occurs within the `AblCodeGenerator.cs` file, and begins at the `AblCodeGenerator.BuildCode()` function, which first generates the ABL agent code, then builds the ABL agent source, copying existing static source files and executing the java and javac binaries that come with the WideRuled2 source. 
* The `CompressWith7ZipForRelease.bat` file in the **`.\src_dotnet\WideRuled2\bin\`** directory takes the Release folder and compresses it with 7zip (assuming it is installed on your computer). 
* When you compile the .NET interface in Debug mode, the "Debug" menu will appear at the top of Wide Ruled 2. This interface allows you launch a tool that requires you to select a sample story for reference, then analyze one or more other stories, resulting in a comma-separated list of statistics about the analyzed stories, along with their similarity to the sample story. The resulting data appears in the window for copy/paste, or can be saved to a file using the "Save Data" button. 
