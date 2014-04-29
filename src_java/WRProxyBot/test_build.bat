:: This file will build build a sample generated ABL agent from the .NET interface, along with all the 
:: supporting Java code. It also gives a sample of the build process that occurs within Wide Ruled 2's .NET interface
:: (not including the actual generation of the .abl file)

:: The purpose of this build is to catch compilation errors in a more friendly (isolated) way during development, instead of attempting to
:: intercept the build process inside of Wide Ruled 2's .NET code. 

set BASEDEV=.

set ABL=%BASEDEV%\lib\abl.jar
set HOJ=%BASEDEV%\lib\hoj.jar

set ABLPATH=%ABL%;%HOJ%

set BUILDDIR=%BASEDEV%\bin

mkdir %BASEDEV%\bin

:: Building WME prerequisites for ABL Code Generation ...
javac -classpath "%ABLPATH%" -d %BUILDDIR% %BASEDEV%\src\javacode\*WME.java

::Do ABL Code Generation, based on a sample ABL file generated from our .NET interface
cd %BASEDEV%\sample_WR2_generated_agent\
java -classpath "..\%BUILDDIR%\;..\%ABL%;..\%HOJ%" abl.compiler.Abl .\*.abl
cd ..

:: Copy Java source support files into same directory as generated ABL java code
XCOPY /Y  %BASEDEV%\src\javacode\*.java %BASEDEV%\sample_WR2_generated_agent\javacode\ 

:: Now build the ABL generated agent code along with the rest of the java support code all at once
javac -classpath "%ABLPATH%" -d %BUILDDIR% %BASEDEV%\sample_WR2_generated_agent\javacode\*.java



