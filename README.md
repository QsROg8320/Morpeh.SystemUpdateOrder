
# Morpeh.SystemUpdateOrder
This extension of Morpeh allows the use `[UpdateBefore]` and `[UpdateAfter]` attributes to specify the system update  order. 

> **Important!** `Odin Inspector` needs to be installed.

# Instalation
Copy the repository files to your project
# Getting Started

 - Add SystemA and SystemB

	```csharp
	[UpdateBefore(typeof(SystemB))]
	public class SystemA : UpdateSystem{

	}

	public class SystemB : UpdateSystem{

	}
	```

 - Right click in hierarchy window and select `ECS/InstallerWithSorting`.

	> If  the checkbox `Include All Systems By Default` is checked, all
	> systems in the assembly would be added to the list. If  the checkbox
	> `Include All Systems By Default` is unchecked, only systems with the
	> attribute `[Include]`  would be added to the list.

 - Specify the path where the scriptable objects of the systems will be stored.
 - Press button  `Sort System`

 
