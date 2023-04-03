# mediatr-navigator
A Visual Studio extension that helps with navigating to the mediatr handlers. Extension provides an option in the code context menu to navigate from a command to the handler of the command.

# Usage
If you right-click on the command identifier, the option '**Go to Handler**' will appear in the code context menu. This option will appear if the extension successfully finds command. This will happend in the following scenarios:
- Right-click on ```_mediatr.Send(command)``` invocation expression
- Right-click on the command as method parameter
- Right-click anywhere inside the command class


For example:

![Screenshot (219)](https://user-images.githubusercontent.com/10000048/229503550-9c6c1d69-848e-4a14-b960-2ba30f4ef448.png)