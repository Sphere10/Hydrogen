namespace Hydrogen.Windows.Forms;

public class MinimizedStartupFolderAutoRunServicesProvider : StartupFolderAutoRunServicesProvider {
	protected override ShellLink.LinkDisplayMode GetShortcutDisplayMode() => ShellLink.LinkDisplayMode.Minimized;
}
