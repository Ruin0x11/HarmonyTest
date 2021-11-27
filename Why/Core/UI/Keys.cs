﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public enum Keys : int
    {
        None = 0,

        // Modifiers
        Ctrl         = 0b0001000000000,
        Alt          = 0b0010000000000,
        Shift        = 0b0100000000000,
        GUI          = 0b1000000000000,
        AllModifiers = 0b1111000000000,

        // Keys (compatible with LOVE)
        Unknown = 0,
        Return = 1,
        Enter = 1,
        Escape = 2,
        Backspace = 3,
        Tab = 4,
        Space = 5,
        Exclaim = 6,
        Quotedbl = 7,
        Hash = 8,
        Percent = 9,
        Dollar = 10,
        Ampersand = 11,
        Quote = 12,
        LeftParen = 13,
        RightParen = 14,
        Asterisk = 0xF,
        Plus = 0x10,
        Comma = 17,
        Minus = 18,
        Period = 19,
        Slash = 20,
        Number0 = 21,
        Number1 = 22,
        Number2 = 23,
        Number3 = 24,
        Number4 = 25,
        Number5 = 26,
        Number6 = 27,
        Number7 = 28,
        Number8 = 29,
        Number9 = 30,
        Colon = 0x1F,
        SemiColon = 0x20,
        Less = 33,
        Equals = 34,
        Greater = 35,
        Question = 36,
        At = 37,
        LeftBracket = 38,
        Backslash = 39,
        RightBracket = 40,
        Caret = 41,
        Underscore = 42,
        Backquote = 43,
        A = 44,
        B = 45,
        C = 46,
        D = 47,
        E = 48,
        F = 49,
        G = 50,
        H = 51,
        I = 52,
        J = 53,
        K = 54,
        L = 55,
        M = 56,
        N = 57,
        O = 58,
        P = 59,
        Q = 60,
        R = 61,
        S = 62,
        T = 0x3F,
        U = 0x40,
        V = 65,
        W = 66,
        X = 67,
        Y = 68,
        Z = 69,
        CapsLock = 70,
        F1 = 71,
        F2 = 72,
        F3 = 73,
        F4 = 74,
        F5 = 75,
        F6 = 76,
        F7 = 77,
        F8 = 78,
        F9 = 79,
        F10 = 80,
        F11 = 81,
        F12 = 82,
        PrintScreen = 83,
        ScrollLock = 84,
        Pause = 85,
        Insert = 86,
        Home = 87,
        PageUp = 88,
        Delete = 89,
        End = 90,
        PageDown = 91,
        Right = 92,
        Left = 93,
        Down = 94,
        Up = 95,
        NumLockClear = 96,
        KeypadDivide = 97,
        KeypadMultiply = 98,
        KeypadMinus = 99,
        KeypadPlus = 100,
        KeypadEnter = 101,
        Keypad1 = 102,
        Keypad2 = 103,
        Keypad3 = 104,
        Keypad4 = 105,
        Keypad5 = 106,
        Keypad6 = 107,
        Keypad7 = 108,
        Keypad8 = 109,
        Keypad9 = 110,
        Keypad0 = 111,
        KeypadPeriod = 112,
        KeypadComma = 113,
        KeypadEquals = 114,
        Application = 115,
        Power = 116,
        F13 = 117,
        F14 = 118,
        F15 = 119,
        F16 = 120,
        F17 = 121,
        F18 = 122,
        F19 = 123,
        F20 = 124,
        F21 = 125,
        F22 = 126,
        F23 = 0x7F,
        F24 = 0x80,
        Execute = 129,
        Help = 130,
        Menu = 131,
        Select = 132,
        Stop = 133,
        Again = 134,
        Undo = 135,
        Cut = 136,
        Copy = 137,
        Paste = 138,
        Find = 139,
        Mute = 140,
        VolumeUp = 141,
        VolumeDown = 142,
        Alterase = 143,
        Sysreq = 144,
        Cancel = 145,
        Clear = 146,
        Prior = 147,
        Return2 = 148,
        Separator = 149,
        Out = 150,
        Oper = 151,
        ClearAgain = 152,
        ThousandsSeparator = 153,
        DecimalSeparator = 154,
        CurrencyUnit = 155,
        CurrencySubunit = 156,
        LCtrl = 157,
        LShift = 158,
        LAlt = 159,
        LGUI = 160,
        RCtrl = 161,
        RShift = 162,
        RAlt = 163,
        RGUI = 164,
        Mode = 165,
        AudioNext = 166,
        AudioPrev = 167,
        AudioStop = 168,
        AudioPlay = 169,
        AudioMute = 170,
        MediaSelect = 171,
        WWW = 172,
        Mail = 173,
        Calculator = 174,
        Computer = 175,
        AppSearch = 176,
        AppHome = 177,
        AppBack = 178,
        AppForward = 179,
        AppStop = 180,
        AppRefresh = 181,
        AppBookmarks = 182,
        BrightnessDown = 183,
        BrightnessUp = 184,
        DisplaySwitch = 185,
        KBDILLUMToggle = 186,
        KBDILLUMDown = 187,
        KBDILLUMUp = 188,
        Eject = 189,
        Sleep = 190
    }
}
