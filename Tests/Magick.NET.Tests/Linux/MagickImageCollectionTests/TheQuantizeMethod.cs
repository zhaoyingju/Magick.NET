﻿// Copyright 2013-2019 Dirk Lemstra <https://github.com/dlemstra/Magick.NET/>
//
// Licensed under the ImageMagick License (the "License"); you may not use this file except in
// compliance with the License. You may obtain a copy of the License at
//
//   https://www.imagemagick.org/script/license.php
//
// Unless required by applicable law or agreed to in writing, software distributed under the
// License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
// either express or implied. See the License for the specific language governing permissions
// and limitations under the License.

#if WINDOWS_BUILD // Cannot detect difference between macOS and Linux build at the moment

using ImageMagick;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Magick.NET.Tests
{
    public partial class MagickImageCollectionTests
    {
        public partial class TheQuantizeMethod
        {
            [TestMethod]
            public void ShouldReduceTheColors()
            {
                using (IMagickImageCollection collection = new MagickImageCollection())
                {
                    collection.Add(Files.FujiFilmFinePixS1ProJPG);

                    QuantizeSettings settings = new QuantizeSettings
                    {
                        Colors = 3,
                    };

                    collection.Quantize(settings);

#if Q8
                    ColorAssert.AreEqual(new MagickColor("#2a424e"), collection[0], 64, 79);
                    ColorAssert.AreEqual(new MagickColor("#7a929d"), collection[0], 76, 153);
                    ColorAssert.AreEqual(new MagickColor("#43729e"), collection[0], 188, 135);
#elif Q16 || Q16HDRI
                    ColorAssert.AreEqual(new MagickColor("#2a1c41e44e3a"), collection[0], 188, 135);
                    ColorAssert.AreEqual(new MagickColor("#7a1e92c39df8"), collection[0], 76, 158);
                    ColorAssert.AreEqual(new MagickColor("#43ab72f09e6b"), collection[0], 66, 115);
#else
#error Not implemented!
#endif
                }
            }

            [TestMethod]
            public void ShouldReturnErrorInfoWhenMeasureErrorsIsTrue()
            {
                using (IMagickImageCollection collection = new MagickImageCollection())
                {
                    collection.Add(Files.FujiFilmFinePixS1ProJPG);

                    QuantizeSettings settings = new QuantizeSettings
                    {
                        Colors = 3,
                        MeasureErrors = true,
                    };

                    MagickErrorInfo errorInfo = collection.Quantize(settings);
                    Assert.IsNotNull(errorInfo);

#if Q8
                    Assert.AreEqual(13.47, errorInfo.MeanErrorPerPixel, 0.01);
                    Assert.AreEqual(0.47, errorInfo.NormalizedMaximumError, 0.01);
                    Assert.AreEqual(0.006, errorInfo.NormalizedMeanError, 0.001);
#elif Q16 || Q16HDRI
                    Assert.AreEqual(3505, errorInfo.MeanErrorPerPixel, 1);
                    Assert.AreEqual(0.47, errorInfo.NormalizedMaximumError, 0.01);
                    Assert.AreEqual(0.006, errorInfo.NormalizedMeanError, 0.001);
#else
#error Not implemented!
#endif
                }
            }
        }
    }
}

#endif