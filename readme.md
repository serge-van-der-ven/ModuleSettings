# XEC DNN ModuleSettings demo

## Introduction

This small DNN 7+ demo module demonstrates the usages of (property) attributes to retrieve and persist ModuleSettings, TabModuleSettings and PortalSettings using a stronly typed POCO.

## Objective

The actual (Module/TabModule/Portal) settings handling is within the Entities folder of the project and should (could) be incorporated in DNN Platform. A module developer should only be 
concerned with the POCO class - in this example "MyModuleSettingsInfo" and the code to load and save settings with this class as demonstrated in the View and Settings controls of this project.
The persister could also be incorporated in DNN Platform using a generic PortalModuleBase variant, e.g. PortalModuleBase<TType> where TType is the settings POCO. This variant could automatically 
load the settings into a strongly typed property of type TType.

## IParameterGrouping

This project contains an IParameterGrouping interface which can be used to group parameters / properties in e.g. the user interface which a category identifier. Also parameter names can 
be prefixed (preferrably using a sub class) within the storage container. However the ParameterGrouping function is not fully implemented in this demo module.

Feel free to comment or leave suggestions.

Serge