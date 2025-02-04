﻿#region license

// Copyright (c) 2021, andreakarasho
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. All advertising materials mentioning features or use of this software
//    must display the following acknowledgement:
//    This product includes software developed by andreakarasho - https://github.com/andreakarasho
// 4. Neither the name of the copyright holder nor the
//    names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS ''AS IS'' AND ANY
// EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER BE LIABLE FOR ANY
// DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
// ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using ClassicUO.Configuration;
using ClassicUO.Game.Data;
using ClassicUO.Renderer;
using ClassicUO.Resources;
using ClassicUO.Utility;
using Microsoft.Xna.Framework;
using MathHelper = Microsoft.Xna.Framework.MathHelper;

namespace ClassicUO.Game
{
    enum WEATHER_TYPE
    {
        WT_RAIN = 0,
        WT_FIERCE_STORM,
        WT_SNOW,
        WT_STORM,

        WT_INVALID_0 = 0xFE,
        WT_INVALID_1 = 0xFF
    }

    internal class Weather
    {
        private const int MAX_WEATHER_EFFECT = 70;
        private const float SIMULATION_TIME = 37.0f;

        private readonly WeatherEffect[] _effects = new WeatherEffect[MAX_WEATHER_EFFECT];
        private uint _timer, _windTimer, _lastTick;


        public WEATHER_TYPE? CurrentWeather { get; private set; }
        public WEATHER_TYPE Type { get; private set; }
        public byte Count { get; private set; }
        public byte CurrentCount { get; private set; }
        public byte Temperature{ get; private set; }
        public sbyte Wind { get; private set; }



        private static float SinOscillate(float freq, int range, uint current_tick)
        {
            float anglef = (int) (current_tick / 2.7777f * freq) % 360;

            return Math.Sign(MathHelper.ToRadians(anglef)) * range;
        }


        public void Reset()
        {
            Type = 0;
            Count = CurrentCount = Temperature = 0;
            Wind = 0;
            _windTimer = _timer = 0;
            CurrentWeather = null;
        }

        public void Generate(WEATHER_TYPE type, byte count, byte temp)
        {
            if (CurrentWeather.HasValue && CurrentWeather == type)
            {
                return;
            }

            Reset();

            Type = type;
            Count = (byte) Math.Min(MAX_WEATHER_EFFECT, (int) count);
            Temperature = temp;
            _timer = Time.Ticks + Constants.WEATHER_TIMER;

            _lastTick = Time.Ticks;

            if (Type == WEATHER_TYPE.WT_INVALID_0 || Type == WEATHER_TYPE.WT_INVALID_1)
            {
                _timer = 0;
                CurrentWeather = null;

                return;
            }

            bool showMessage = Count > 0;

            switch (type)
            {
                case WEATHER_TYPE.WT_RAIN:
                    if (showMessage)
                    {
                        GameActions.Print
                        (
                            ResGeneral.ItBeginsToRain,
                            1154,
                            MessageType.System,
                            3,
                            false
                        );

                        CurrentWeather = type;
                    }

                    break;

                case WEATHER_TYPE.WT_FIERCE_STORM:
                    if (showMessage)
                    {
                        GameActions.Print
                        (
                            ResGeneral.AFierceStormApproaches,
                            1154,
                            MessageType.System,
                            3,
                            false
                        );

                        CurrentWeather = type;
                    }

                    break;

                case WEATHER_TYPE.WT_SNOW:
                    if (showMessage)
                    {
                        GameActions.Print
                        (
                            ResGeneral.ItBeginsToSnow,
                            1154,
                            MessageType.System,
                            3,
                            false
                        );

                        CurrentWeather = type;
                    }

                    break;

                case WEATHER_TYPE.WT_STORM:
                    if (showMessage)
                    {
                        GameActions.Print
                        (
                            ResGeneral.AStormIsBrewing,
                            1154,
                            MessageType.System,
                            3,
                            false
                        );

                        CurrentWeather = type;
                    }

                    break;
            }


            _windTimer = 0;

            while (CurrentCount < Count)
            {
                ref WeatherEffect effect = ref _effects[CurrentCount++];
                effect.X = RandomHelper.GetValue(0, ProfileManager.CurrentProfile.GameWindowSize.X);
                effect.Y = RandomHelper.GetValue(0, ProfileManager.CurrentProfile.GameWindowSize.Y);
            }
        }

        public void Draw(UltimaBatcher2D batcher, int x, int y)
        {
            bool removeEffects = false;

            if (_timer < Time.Ticks)
            {
                if (CurrentCount == 0)
                {
                    return;
                }

                removeEffects = true;
            }
            else if (Type == WEATHER_TYPE.WT_INVALID_0 || Type == WEATHER_TYPE.WT_INVALID_1)
            {
                return;
            }

            uint passed = Time.Ticks - _lastTick;

            if (passed > 7000)
            {
                _lastTick = Time.Ticks;
                passed = 25;
            }

            bool windChanged = false;

            if (_windTimer < Time.Ticks)
            {
                if (_windTimer == 0)
                {
                    windChanged = true;
                }

                _windTimer = Time.Ticks + (uint) (RandomHelper.GetValue(7, 13) * 1000);

                sbyte lastWind = Wind;

                Wind = (sbyte) RandomHelper.GetValue(0, 4);

                if (RandomHelper.GetValue(0, 2) != 0)
                {
                    Wind *= -1;
                }

                if (Wind < 0 && lastWind > 0)
                {
                    Wind = 0;
                }
                else if (Wind > 0 && lastWind < 0)
                {
                    Wind = 0;
                }

                if (lastWind != Wind)
                {
                    windChanged = true;
                }
            }

            //switch ((WEATHER_TYPE) Type)
            //{
            //    case WEATHER_TYPE.WT_RAIN:
            //    case WEATHER_TYPE.WT_FIERCE_STORM:
            //        // TODO: set color
            //        break;
            //    case WEATHER_TYPE.WT_SNOW:
            //    case WEATHER_TYPE.WT_STORM:
            //        // TODO: set color
            //        break;
            //    default:
            //        break;
            //}

            //Point winpos = ProfileManager.CurrentProfile.GameWindowPosition;
            Point winsize = ProfileManager.CurrentProfile.GameWindowSize;

            Rectangle rect = new Rectangle(0, 0, 2, 2);

            for (int i = 0; i < CurrentCount; i++)
            {
                ref WeatherEffect effect = ref _effects[i];

                if (effect.X < x || effect.X > x + winsize.X || effect.Y < y || effect.Y > y + winsize.Y)
                {
                    if (removeEffects)
                    {
                        if (CurrentCount > 0)
                        {
                            CurrentCount--;
                        }
                        else
                        {
                            CurrentCount = 0;
                        }

                        continue;
                    }

                    effect.X = x + RandomHelper.GetValue(0, winsize.X);
                    effect.Y = y + RandomHelper.GetValue(0, winsize.Y);
                }


                switch (Type)
                {
                    case WEATHER_TYPE.WT_RAIN:
                        float scaleRation = effect.ScaleRatio;
                        effect.SpeedX = -4.5f - scaleRation;
                        effect.SpeedY = 5.0f + scaleRation;

                        break;

                    case WEATHER_TYPE.WT_FIERCE_STORM:
                        effect.SpeedX = Wind;
                        effect.SpeedY = 6.0f;

                        break;

                    case WEATHER_TYPE.WT_SNOW:
                    case WEATHER_TYPE.WT_STORM:

                        if (Type == WEATHER_TYPE.WT_SNOW)
                        {
                            effect.SpeedX = Wind;
                            effect.SpeedY = 1.0f;
                        }
                        else
                        {
                            effect.SpeedX = Wind * 1.5f;
                            effect.SpeedY = 1.5f;
                        }

                        if (windChanged)
                        {
                            effect.SpeedAngle = MathHelper.ToDegrees((float) Math.Atan2(effect.SpeedX, effect.SpeedY));

                            effect.SpeedMagnitude = (float) Math.Sqrt(Math.Pow(effect.SpeedX, 2) + Math.Pow(effect.SpeedY, 2));
                        }

                        float speed_angle = effect.SpeedAngle;
                        float speed_magnitude = effect.SpeedMagnitude;

                        speed_magnitude += effect.ScaleRatio;

                        speed_angle += SinOscillate(0.4f, 20, Time.Ticks + effect.ID);

                        float rad = MathHelper.ToRadians(speed_angle);
                        effect.SpeedX = speed_magnitude * (float) Math.Sin(rad);
                        effect.SpeedY = speed_magnitude * (float) Math.Cos(rad);

                        break;
                }

                float speedOffset = passed / SIMULATION_TIME;

                switch ((WEATHER_TYPE) Type)
                {
                    case WEATHER_TYPE.WT_RAIN:
                    case WEATHER_TYPE.WT_FIERCE_STORM:

                        int oldX = (int) effect.X;
                        int oldY = (int) effect.Y;

                        float ofsx = effect.SpeedX * speedOffset;
                        float ofsy = effect.SpeedY * speedOffset;

                        effect.X += ofsx;
                        effect.Y += ofsy;

                        const float MAX_OFFSET_XY = 5.0f;

                        if (ofsx >= MAX_OFFSET_XY)
                        {
                            oldX = (int) (effect.X - MAX_OFFSET_XY);
                        }
                        else if (ofsx <= -MAX_OFFSET_XY)
                        {
                            oldX = (int) (effect.X + MAX_OFFSET_XY);
                        }

                        if (ofsy >= MAX_OFFSET_XY)
                        {
                            oldY = (int) (effect.Y - MAX_OFFSET_XY);
                        }
                        else if (oldY <= -MAX_OFFSET_XY)
                        {
                            oldY = (int) (effect.Y + MAX_OFFSET_XY);
                        }

                        Vector2 start = new Vector2(x + oldX, y + oldY);
                        Vector2 end = new Vector2(x + effect.X, y + effect.Y);

                        batcher.DrawLine
                        (
                           SolidColorTextureCache.GetTexture(Color.Gray),
                           start,
                           end,
                           Vector3.Zero,
                           1
                        );

                        break;

                    case WEATHER_TYPE.WT_SNOW:
                    case WEATHER_TYPE.WT_STORM:

                        effect.X += effect.SpeedX * speedOffset;
                        effect.Y += effect.SpeedY * speedOffset;

                        rect.X = x + (int) effect.X;
                        rect.Y = y + (int) effect.Y;

                        batcher.Draw
                        (
                            SolidColorTextureCache.GetTexture(Color.White),
                            rect,
                            Vector3.Zero
                        );

                        break;
                }
            }

            _lastTick = Time.Ticks;
        }


        private struct WeatherEffect
        {
            public float SpeedX, SpeedY, X, Y, ScaleRatio, SpeedAngle, SpeedMagnitude;
            public uint ID;
        }
    }
}