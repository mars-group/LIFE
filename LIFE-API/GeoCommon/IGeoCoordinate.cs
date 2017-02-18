// // /*******************************************************
// //  * Copyright (C) Christian Hüning - All Rights Reserved
// //  * Unauthorized copying of this file, via any medium is strictly prohibited
// //  * Proprietary and confidential
// //  * This file is part of the MARS LIFE project, which is part of the MARS System
// //  * More information under: http://www.mars-group.org
// //  * Written by Christian Hüning <christianhuening@gmail.com>, 30.07.2016
// //  *******************************************************/
using System;

namespace LIFE.API.GeoCommon {

  public interface IGeoCoordinate : IEquatable<IGeoCoordinate> {
    double Latitude { get; set; }
    double Longitude { get; set; }
  }
}