# 360 Soundscape Player with Survey

This Unity based project will enable users to play 360 videos and also compare different audio reproduction methods through an in-built survey.

## Getting Started

While most of the project is based around Unity, a separate DAW (REAPER) is used to translate the audio into a 360 environment. Please follow the steps to link both programs together.

### Prerequisites
1. HTC Vive or other VR controllers with OpenVR/SteamVR support

2. Unity + Assets

```
[Base Client](https://unity3d.com/get-unity/download)
[OSCSimpl](https://assetstore.unity.com/packages/tools/input-management/osc-simpl-53710)
```

3. REAPER + Plugins
```
[Base Client](https://www.reaper.fm/)
[COMPASS VST Plugin](http://research.spa.aalto.fi/projects/compass_vsts/plugins.html)
```

4. HEVC Video Extensions (if .h265 videos don't run)
```
[Extension](https://www.microsoft.com/en-us/p/hevc-video-extensions-from-device-manufacturer/9n4wgh0z6vhq)
```

### Installing
1. Download Unity, OSCSimpl, REAPER and COMPASS in this order and follow their corresponding setups.

2. Download the project and navigate to \soundscape_player\360 Unity Official\Assets\Interactive360\Scripts

3. Edit both MultiChoiceQns.cs and MushraQns.cs to reflect where the below files are located. They are usually in where ever the project was downloaded to.
     1. ENVIRONMENT: "multiAns.txt" and "multiQns.txt"
     2. MUSHRA: "mushraAns.txt" and mushraQns.txt"
     
4. To insert videos into the scene, please copy and paste the desired videos into this path:
   path\soundscape-player\Unity\360 Unity Official\Assets\Interactive360\Videos
   
5. Audio files can be used from anywhere but must be specified within REAPER

6. Template REAPER project file can be opened from these locations.
     1. ENVIRONMENT: \Audio Comparisons\Env Comparison\Env_comparison_mix.rpp
     2. MUSHRA: \Audio Comparisons\MUSHRA Comparison\Comparison_mix_streamlined.rpp
     
7. When opening REAPER, press "Ignore all missing files". This allows you to substitute the files with your own audio files.

8. When first starting the application, start MainMenu_Controller scene to play.

9. Press the button above the D-pad on the Vive controller to start the survey interface.
          

## Authors

* **Tan Yi Xian** - *Initial work* - [ytan101](https://github.com/PurpleBooth)

See also the list of [contributors](https://github.com/your/project/contributors) who participated in this project.
