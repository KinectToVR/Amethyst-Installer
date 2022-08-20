# Amethyst Installer

This is the installer for [Amethyst](https://github.com/KinectToVR/Amethyst), and it installs from the [releases repo](https://github.com/KinectToVR/Amethyst-Releases).

The installer is currently in an **UNFINISHED** state, and actively being developed.

## Translations

If you would like to contribute to this project by translating strings for other languages, you may do the following:

1. Download the latest installer from Releases.

2. Throw the installer into a folder of your choice.

3. Create a folder called "Lang" in this folder.

4. Download [this file](https://raw.githubusercontent.com/KinectToVR/Amethyst-Installer/main/Amethyst-Installer/Resources/Lang/en.json) to the "Lang" folder, and rename it to "locale.json" (make sure that you have file extensions enabled)

5. Start the installer in debug mode (this will allow you to navigate to other pages easily to better see what text will look like, and load your copy of "locale.json" instead of the built-in translations):
   
   ```cmd
   Amethyst-Installer.exe --debug
   ```

6. File an issue or create a pull request to request us to add your translations to the installer. Alternatively, contact us on [the Discord server](https://discord.gg/YBQCRDG) in #development to get your translations merged.