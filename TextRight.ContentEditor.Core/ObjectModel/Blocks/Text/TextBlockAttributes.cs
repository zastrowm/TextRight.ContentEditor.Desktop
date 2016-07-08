﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace TextRight.ContentEditor.Core.ObjectModel.Blocks
{
  /// <summary> A class that contains the required attributes of a TextBlock's attributes. </summary>
  public abstract class TextBlockAttributes
  {
    public abstract TextBlock CreateInstance();
  }
}