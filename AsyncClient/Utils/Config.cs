﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AsyncClient.Utils
{
    public static class Config
    {
        internal const int WIN_WIDTH = 100;
        internal const int WIN_HEIGHT = 41;
        internal const int IP_W = 80;
        internal const int IP_H = 0;
        internal const int ALERT_W = 0;
        internal const int ALERT_H = 0;
        internal const int MESSBOX_START_W = 0;
        internal const int MESSBOX_END_W = 100;
        internal const int MESSBOX_START_H = 2;
        internal const int MESSBOX_END_H = 37;
        internal const int ASK_W = 0;
        internal const int ASK_H = 39;

        internal const ConsoleColor MESSBOX_COLOR_BACK = ConsoleColor.Black;
        internal const ConsoleColor MESSBOX_COLOR_FRONT = ConsoleColor.Cyan;


        internal const int MESSAGE_STACK_MAX_LIN = 37;
        internal const int MESSAGE_STACK_MAX_COL = 100;

    }

    public static class Commands
    {
        internal const string EXIT = "$exit";
        internal const string NICK = "$nick";
        internal const string TIME = "$gettime";
    }
}