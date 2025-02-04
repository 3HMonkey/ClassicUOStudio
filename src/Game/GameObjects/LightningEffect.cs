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

using ClassicUO.Game.Managers;

namespace ClassicUO.Game.GameObjects
{
    internal sealed partial class LightningEffect : GameEffect
    {
        public LightningEffect(EffectManager manager, uint src, int x, int y, int z, ushort hue) 
            : base(manager, 0x4E20, hue, 400, 0)
        {
            IsEnabled = true;
            AnimIndex = 0;

            Entity source = World.Get(src);

            if (SerialHelper.IsValid(src) && source != null)
            {
                SetSource(source);
            }
            else
            {
                SetSource(x, y, z);
            }
        }

        public override void Update(double totalTime, double frameTime)
        {
            if (!IsDestroyed)
            {
                if (AnimIndex >= 10 || (Duration < totalTime && Duration >= 0))
                {
                    Destroy();
                }
                else
                {
                    AnimationGraphic = (ushort) (Graphic + AnimIndex);

                    if (NextChangeFrameTime < totalTime)
                    {
                        AnimIndex++;
                        NextChangeFrameTime = (long) totalTime + IntervalInMs;
                    }

                    (int x, int y, int z) = GetSource();

                    if (X != x || Y != y || Z != z)
                    {
                        X = (ushort) x;
                        Y = (ushort) y;
                        Z = (sbyte) z;
                        UpdateScreenPosition();
                        AddToTile();
                    }
                }
            }
        }
    }
}