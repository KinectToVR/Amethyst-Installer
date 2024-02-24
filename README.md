# Amethyst Installer


### How do I install or update Amethyst?
[<img width="290px" src="https://get.microsoft.com/images/en-us%20light.svg">](https://www.microsoft.com/store/apps/9P7R8FGDDGDH)

**Scroll further down for manual instructions.**  

Amethyst is a Windows application for using various devices for body tracking in virtual reality. It can be [extended with user-made plugins](https://docs.k2vr.tech/en/dev/overview) to support any device you wish.

This is a rewrite from the ground up, it is *not* based on KinectToVR/K2EX. It is a whole new app that doesn't carry the legacy baggage of K2EX. We were able to fix numerous bugs and streamline the experience. We hope you will enjoy it. If you like what you see and you wish to support future development, you can throw money at us with the [<img style="display:inline; height:26px;" src="https://user-images.githubusercontent.com/8508676/189487326-eff20178-77a2-4ea4-9798-d389e53501e4.png">](https://opencollective.com/k2vr) button.
  (We won't force you, though every expense is currently out of pocket.)

## Manual setup

If the Microsoft Store isn't available for you, for some reason:
 - **Download 11835K2VRTeam.Amethyst_[...]_.Msix** from the [latest release](https://github.com/KinectToVR/Amethyst-Releases/releases/latest).
 - **You're good to go!** Just go through the setup and start playing!

---

This repository holds the published releases for Amethyst Installer.

Amethyst Installer has been shut down in favor of Microsoft Store deployment and an integrated OOBE Setup.
> As of August 9th 2023 Amethyst has finally been moved to the Microsoft Store, and is now at 1.2.5.0!

This build of the Installer will only continue to show this notice and guide you to the Microsoft Store instead.
If, for some reason, you want to use the Installer, please refer to [the docs](https://docs.k2vr.tech) for its launch uri's and generics.

This is the installer for [Amethyst](https://github.com/KinectToVR/Amethyst), and it installs from the [releases repo](https://github.com/KinectToVR/Amethyst-Releases).

## For users: [downloads are here](https://github.com/kinecttovr/amethyst-installer-releases)

## Translations

If you would like to contribute to this project by translating strings for other languages, you may do the following:

1. Download the latest installer build from Releases.

2. Move the installer into a folder of your choice.

3. Create a folder called "Lang" in this folder.

4. Download [this file](https://raw.githubusercontent.com/KinectToVR/Amethyst-Installer/main/Amethyst-Installer/Resources/Lang/en.json) to the "Lang" folder, and rename it to "locale.json" (make sure that you have file extensions enabled)

5. Start the installer in debug mode (this will allow you to navigate to other pages easily to better see what text will look like, and load your copy of "locale.json" instead of the built-in translations):
   
   ```cmd
   Amethyst-Installer.exe --debug
   ```

6. File an issue or create a pull request to request us to add your translations to the installer. Alternatively, contact us on [the Discord server](https://discord.gg/YBQCRDG) in #development to get your translations merged.

# Developers

To use debug builds of Amethyst Installer you need to [enable Graphics Debugging in Windows](https://learn.microsoft.com/en-us/windows/uwp/gaming/use-the-directx-runtime-and-visual-studio-graphics-diagnostic-features).
