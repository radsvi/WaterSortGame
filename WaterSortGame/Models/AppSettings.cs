﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterSortGame.Properties;
using WaterSortGame.ViewModels;

namespace WaterSortGame.Models
{
    internal class AppSettings : ViewModelBase
    {
        private bool loadDebugLevel = Settings.Default.LoadDebugLevel;
        public bool LoadDebugLevel
        {
            get { return loadDebugLevel; }
            set
            {
                //if (value != loadDebugLevel)
                //{
                loadDebugLevel = value;
                Settings.Default.LoadDebugLevel = loadDebugLevel;
                Settings.Default.Save();
                //OnPropertyChanged();
                //}
            }
        }
        private int numberOfColorsToGenerate = Settings.Default.NumberOfColorsToGenerate;
        public int NumberOfColorsToGenerate
        {
            get { return numberOfColorsToGenerate; }
            set
            {
                if (numberOfColorsToGenerate != value)
                {
                    if (value >= 3 && value <= LiquidColor.ColorKeys.Count)
                    {
                        numberOfColorsToGenerate = value;
                    }
                    else if (value < 3)
                    {
                        numberOfColorsToGenerate = 3;
                    }
                    else if (value > LiquidColor.ColorKeys.Count)
                    {
                        numberOfColorsToGenerate = LiquidColor.ColorKeys.Count;
                    }
                    //OnPropertyChanged();
                    //OnGlobalPropertyChanged("NumberOfColorsToGenerate");
                    Settings.Default.NumberOfColorsToGenerate = numberOfColorsToGenerate;
                    Settings.Default.Save();
                }
            }
        }
        private bool randomNumberOfTubes = Settings.Default.RandomNumberOfTubes;
        public bool RandomNumberOfTubes
        {
            get { return randomNumberOfTubes; }
            set
            {
                randomNumberOfTubes = value;
                Settings.Default.RandomNumberOfTubes = value;
                Settings.Default.Save();
            }
        }

        private int maximumExtraTubes = Settings.Default.MaximumExtraTubes;
        public int MaximumExtraTubes
        {
            get { return maximumExtraTubes; }
            set
            {
                if (maximumExtraTubes != value)
                {
                    if (value >= 0 && value <= 20)
                    {
                        maximumExtraTubes = value;
                    }
                    else if (value < 0)
                    {
                        maximumExtraTubes = 0;
                    }
                    else if (value > 20)
                    {
                        maximumExtraTubes = 20;
                    }
                    Settings.Default.MaximumExtraTubes = value;
                    Settings.Default.Save();
                }
            }
        }
        private bool dontShowHelpScreenAtStart = Settings.Default.DontShowHelpScreenAtStart;
        public bool DontShowHelpScreenAtStart
        {
            get { return dontShowHelpScreenAtStart; }
            set
            {
                if (value != dontShowHelpScreenAtStart)
                {
                    dontShowHelpScreenAtStart = value;
                    Settings.Default.DontShowHelpScreenAtStart = dontShowHelpScreenAtStart;
                    Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }
        private bool developerOptionsVisibleBool = Settings.Default.DeveloperOptionsVisibleBool;
        public bool DeveloperOptionsVisibleBool
        {
            get { return developerOptionsVisibleBool; }
            set
            {
                if (value != developerOptionsVisibleBool)
                {
                    developerOptionsVisibleBool = value;
                    Settings.Default.DeveloperOptionsVisibleBool = developerOptionsVisibleBool;
                    Settings.Default.Save();
                    OnPropertyChanged(nameof(DeveloperOptionsVisible));
                }
            }
        }
        public string DeveloperOptionsVisible
        {
            get
            {
                if (developerOptionsVisibleBool == true)
                {
                    return "Visible";
                }
                else
                {
                    return "Hidden";
                }
            }

        }
        private bool unselectTubeEvenOnIllegalMove = Settings.Default.UnselectTubeEvenOnIllegalMove;
        public bool UnselectTubeEvenOnIllegalMove
        {
            get { return unselectTubeEvenOnIllegalMove; }
            set
            {
                if (value != unselectTubeEvenOnIllegalMove)
                {
                    unselectTubeEvenOnIllegalMove = value;
                    Settings.Default.UnselectTubeEvenOnIllegalMove = unselectTubeEvenOnIllegalMove;
                    Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }
        private int optionsWindowHeight = Settings.Default.OptionsWindowHeight;
        public int OptionsWindowHeight
        {
            get { return optionsWindowHeight; }
            set
            {
                optionsWindowHeight = value;
                Settings.Default.OptionsWindowHeight = value;
                Settings.Default.Save();
            }
        }
        private int optionsWindowWidth = Settings.Default.OptionsWindowWidth;
        public int OptionsWindowWidth
        {
            get { return optionsWindowWidth; }
            set
            {
                optionsWindowWidth = value;
                Settings.Default.OptionsWindowWidth = value;
                Settings.Default.Save();
            }
        }
    }
}
