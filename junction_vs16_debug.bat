rem ---- DONT RUN THIS as a GIT USER, No need --------------------------------
rem ---- Set up all paths in the project as relative paths - Saab Dev Only ---


rem ------------- GizmoSDK dependencies --------------------------------------

if not defined GIZMOSDK (
  set GIZMOSDK=..\GizmoSDK
)

rmdir /S / Q Assets\Saab\GizmoSDK
mkdir Assets\Saab\GizmoSDK

mklink /J Assets\Saab\GizmoSDK\GizmoBase 		%GIZMOSDK%\GizmoBase\source\C#
mklink /J Assets\Saab\GizmoSDK\GizmoDistribution 	%GIZMOSDK%\GizmoDistribution\source\C#


rem ------------- End of GizmoSDK dependencies -------------------------------


rem ------------- runtime dlls -----------------------------------------------

rmdir /S / Q Assets\Plugins
mkdir Assets\Plugins

mklink /J Assets\Plugins\x64 %GIZMOSDK%\GizmoDistribution\ws\C#\vs16\GizmoDistribution\x64\Debug
