#if !LATER

namespace MultiSelectTree
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    ///  constants/WIN32 method, ported from Native.cs/SafeNativeMethods.cs/UnsafeNativeMethods.cs
    /// </summary>
    public class Native
    {
        public enum MB
        {
            OK = 0x00000000,
            ICONHAND = 0x00000010,
            ICONQUESTION = 0x00000020,
            ICONEXCLAMATION = 0x00000030,
            ICONASTERISK = 0x00000040,
        };

        public enum VK
        {
            PAGEUP = 0x21,
            PAGEDOWN = 0x22,
            END = 0x23,
            HOME = 0x24,
            DOWN = 0x28,
            UP = 0x26,
            SHIFT = 0x10,
            CONTROL = 0x11,
        }

        public enum ERROR
        {
            FILE_NOT_FOUND = 2,
        }

        public enum Msg
        {
            WM_CLOSE = 0x0010,
            WM_USER = 0x0400,
            WM_REFLECT = WM_USER + 0x1c00,
            WM_COMMAND = 0x0111,
            WM_SYSKEYDOWN = 0x0104,
            WM_NOTIFY = 0x004E,
            WM_SETFOCUS = 0x0007,
            WM_KILLFOCUS = 0x0008,
            WM_CREATE = 0x0001,
            WM_DESTROY = 0x0002,
            WM_LBUTTONDOWN = 0x0201,
            WM_RBUTTONDOWN = 0x0204,
            WM_CONTEXTMENU = 0x007B,
            WM_KEYDOWN = 0x0100,
            WM_KEYUP = 0x0101,
            WM_CHAR = 0x0102,
            WM_HSCROLL = 0x0114,
            WM_VSCROLL = 0x0115,
        }

        public enum ReflectedMsg
        {
            WM_NOTIFY = (Msg.WM_REFLECT + Msg.WM_NOTIFY),
        }

        public enum NotifyMsg
        {
            NM_FIRST = (0 - 0),
            NM_CUSTOMDRAW = (NM_FIRST - 12),
            NM_NCHITTEST = (NM_FIRST - 14)
        }

        public const int CBN_DROPDOWN = 0x0007;
        public const int CBN_CLOSEUP = 0x0008;

        public const int TVIF_STATE = 0x0008;
        public const int TVIF_HANDLE = 0x0010;
        public const int TVIS_SELECTED = 0x0002;

        public const int TVI_ROOT = (unchecked((int)0xFFFF0000));
        public const int TVI_FIRST = (unchecked((int)0xFFFF0001));
        public const int TVGN_CARET = 0x0009;

        public const int MK_SHIFT = 0x0004;
        public const int MK_CONTROL = 0x0008;

        public const int TVE_COLLAPSE = 0x0001;
        public const int TVE_EXPAND = 0x0002;
        public const int TVE_TOGGLE = 0x0003;
        public const int TVE_EXPANDPARTIAL = 0x4000;
        public const int TVE_COLLAPSERESET = 0x8000;

        public const int TVN_SELCHANGEDA = ((0 - 400) - 2);
        public const int TVN_SELCHANGEDW = ((0 - 400) - 51);
        public const int TVN_SELCHANGINGA = ((0 - 400) - 1);
        public const int TVN_SELCHANGINGW = ((0 - 400) - 50);
        public const int TVN_ITEMEXPANDINGA = ((0 - 400) - 5);
        public const int TVN_ITEMEXPANDINGW = ((0 - 400) - 54);
        public const int TVN_ITEMEXPANDEDA = ((0 - 400) - 6);
        public const int TVN_ITEMEXPANDEDW = ((0 - 400) - 55);
        public const int TVN_DELETEITEMA = ((0 - 400) - 9);
        public const int TVN_DELETEITEMW = ((0 - 400) - 58);

        public const int TVHT_ONITEMBUTTON = 0x0010;
        public const int TVHT_ONITEM = (0x0002 | 0x0004 | 0x0040);

        public const int TVM_DELETEITEM = (0x1100 + 1);
        public const int TVM_SETITEMA = (0x1100 + 13);
        public const int TVM_SETITEMW = (0x1100 + 63);
        public const int TVM_GETITEMA = (0x1100 + 12);
        public const int TVM_GETITEMW = (0x1100 + 62);
        public const int TVM_GETNEXTITEM = (0x1100 + 10);
        public const int TVM_SELECTITEM = (0x1100 + 11);
        public const int TVM_HITTEST = (0x1100 + 17);
        public static readonly int TVM_SETITEM = (Marshal.SystemDefaultCharSize == 1) ? TVM_SETITEMA : TVM_SETITEMW;
        public static readonly int TVM_GETITEM = (Marshal.SystemDefaultCharSize == 1) ? TVM_GETITEMA : TVM_GETITEMW;

        //Edit Box
        internal const int EM_SETCHARFORMAT = ((int)Native.Msg.WM_USER + 68);
        internal const int EM_SETPARAFORMAT = ((int)Native.Msg.WM_USER + 71);

        /* CHARFORMAT masks */
        internal const int CFM_BOLD = 0x00000001;
        internal const int CFM_ITALIC = 0x00000002;
        internal const int CFM_UNDERLINE = 0x00000004;
        internal const int CFM_STRIKEOUT = 0x00000008;
        internal const int CFM_PROTECTED = 0x00000010;
        internal const int CFM_LINK = 0x00000020; /* Exchange hyperlink extension */
        internal const int CFM_SIZE = unchecked((int)0x80000000);
        internal const int CFM_COLOR = 0x40000000;
        internal const int CFM_FACE = 0x20000000;
        internal const int CFM_OFFSET = 0x10000000;
        internal const int CFM_CHARSET = 0x08000000;

        // CHARFORMAT2 masks
        internal const int CFM_SMALLCAPS = 0x0040;
        internal const int CFM_ALLCAPS = 0x0080; /* Displayed by 3.0    */
        internal const int CFM_HIDDEN = 0x0100; /* Hidden by 3.0 */
        internal const int CFM_OUTLINE = 0x0200;
        internal const int CFM_SHADOW = 0x0400;
        internal const int CFM_EMBOSS = 0x0800;
        internal const int CFM_IMPRINT = 0x1000;
        internal const int CFM_DISABLED = 0x2000;
        internal const int CFM_REVISED = 0x4000;

        internal const int CFM_BACKCOLOR = 0x04000000;
        internal const int CFM_LCID = 0x02000000;
        internal const int CFM_UNDERLINETYPE = 0x00800000; /* Many displayed by 3.0 */
        internal const int CFM_WEIGHT = 0x00400000;
        internal const int CFM_SPACING = 0x00200000; /* Displayed by 3.0    */
        internal const int CFM_KERNING = 0x00100000;
        internal const int CFM_STYLE = 0x00080000;
        internal const int CFM_ANIMATION = 0x00040000;
        internal const int CFM_REVAUTHOR = 0x00008000;

        internal const int CFM_SUBSCRIPT = CFE_SUBSCRIPT | CFE_SUPERSCRIPT;
        internal const int CFM_SUPERSCRIPT = CFM_SUBSCRIPT;

        // CHARFORMAT effects
        internal const int CFE_BOLD = 0x0001;
        internal const int CFE_ITALIC = 0x0002;
        internal const int CFE_UNDERLINE = 0x0004;
        internal const int CFE_STRIKEOUT = 0x0008;
        internal const int CFE_PROTECTED = 0x0010;
        internal const int CFE_LINK = 0x0020;
        internal const int CFE_AUTOCOLOR = 0x40000000; /* NOTE: this corresponds to */

        // CHARFORMAT2 effects
        internal const int CFE_SUBSCRIPT = 0x00010000; /* Superscript and subscript are */
        internal const int CFE_SUPERSCRIPT = 0x00020000; /*  mutually exclusive             */

        /* CFM_COLOR, which controls it */

        /* EM_SETCHARFORMAT wparam masks */
        internal const int SCF_SELECTION = 0x0001;
        internal const int SCF_WORD = 0x0002;
        internal const int SCF_DEFAULT = 0x0000;        // set the default charformat or paraformat
        internal const int SCF_ALL = 0x0004;        // not valid with SCF_SELECTION or SCF_WORD
        internal const int SCF_USEUIRULES = 0x0008;        // modifier for SCF_SELECTION; says that
        // the format came from a toolbar, etc. and
        // therefore UI formatting rules should be
        // used instead of strictly formatting the
        // selection.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        public class CHARFORMATA
        {
            public int cbSize = Marshal.SizeOf(typeof(CHARFORMATA));
            public int dwMask;
            public int dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szFaceName = new char[32];
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
        public class CHARFORMAT2A
        {
            //CHARFORMATA
            public int cbSize = Marshal.SizeOf(typeof(CHARFORMAT2A));
            public int dwMask;
            public int dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public char[] szFaceName = new char[32];
            //CHARFORMATA2 (additions)
            public short wWeight; /* Font weight (LOGFONT value)              */
            public short sSpacing; /* Amount to space between letters          */
            public int crBackColor; /* Background color                         */
            public int lcid; /* Locale ID                                */
            public int dwReserved; /* Reserved. Must be 0                      */
            public short sStyle; /* Style handle                             */
            public short wKerning; /* Twip size above which to kern char pair  */
            public byte bUnderlineType; /* Underline type                           */
            public byte bAnimation; /* Animated text like marching ants         */
            public byte bRevAuthor; /* Revision author index                    */
        }

        [System.Runtime.InteropServices.ComVisible(false), StructLayout(LayoutKind.Sequential)]
        public class PARAFORMAT2
        {
            //PARAFORMAT
            public int cbSize = 188;   // ndirect.DllLib.sizeOf( this );
            public int dwMask;
            public short wNumbering;
            public short wReserved;
            public int dxStartIndent;
            public int dxRightIndent;
            public int dxOffset;
            public short wAlignment;
            public short cTabCount;

            [MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 32)]
            public int[] rgxTabs;

            //PARAFORMAT2
            public int dySpaceBefore; /* Vertical spacing before para             */
            public int dySpaceAfter; /* Vertical spacing after para              */
            public int dyLineSpacing; /* Line spacing depending on Rule           */
            public short sStyle; /* Style handle                             */
            public byte bLineSpacingRule; /* Rule for line spacing (see tom.doc)      */
            public byte bOutlineLevel; /* Outline Level                            */
            public short wShadingWeight; /* Shading in hundredths of a per cent      */
            public short wShadingStyle; /* Byte 0: style, nib 2: cfpat, 3: cbpat    */
            public short wNumberingStart; /* Starting value for numbering             */
            public short wNumberingStyle; /* Alignment, Roman/Arabic, (), ), ., etc.  */
            public short wNumberingTab; /* Space bet 1st indent and 1st-line text   */
            public short wBorderSpace; /* Border-text spaces (nbl/bdr in pts)      */
            public short wBorderWidth; /* Pen widths (nbl/bdr in half twips)       */
            public short wBorders; /* Border styles (nibble/border)            */
        }

        /* PARAFORMAT 2.0 masks */
        internal const int PFM_STARTINDENT = 0x00000001;
        internal const int PFM_RIGHTINDENT = 0x00000002;
        internal const int PFM_OFFSET = 0x00000004;
        internal const int PFM_ALIGNMENT = 0x00000008;
        internal const int PFM_TABSTOPS = 0x00000010;
        internal const int PFM_NUMBERING = 0x00000020;
        internal const int PFM_OFFSETINDENT = unchecked((int)0x80000000);

        /* PARAFORMAT 2.0 masks and effects */
        internal const int PFM_SPACEBEFORE = 0x00000040;
        internal const int PFM_SPACEAFTER = 0x00000080;
        internal const int PFM_LINESPACING = 0x00000100;
        internal const int PFM_STYLE = 0x00000400;
        internal const int PFM_BORDER = 0x00000800;
        internal const int PFM_SHADING = 0x00001000;
        internal const int PFM_NUMBERINGSTYLE = 0x00002000;
        internal const int PFM_NUMBERINGTAB = 0x00004000;
        internal const int PFM_NUMBERINGSTART = 0x00008000;

        internal const int PFM_RTLPARA = 0x00010000;
        internal const int PFM_KEEP = 0x00020000;
        internal const int PFM_KEEPNEXT = 0x00040000;
        internal const int PFM_PAGEBREAKBEFORE = 0x00080000;
        internal const int PFM_NOLINENUMBER = 0x00100000;
        internal const int PFM_NOWIDOWCONTROL = 0x00200000;
        internal const int PFM_DONOTHYPHEN = 0x00400000;
        internal const int PFM_SIDEBYSIDE = 0x00800000;

        internal const int PFM_TABLE = unchecked((int)0xc0000000);

        /* PARAFORMAT numbering options */
        public enum ParagraphNumberingStyle
        {
            None = 0x0000,
            Bullet = 0x0001
        };

        //ScrollBar
        public const int SB_THUMBPOSITION = 4;

        // Custom Draw State Flags
        public enum CustomDrawDrawStateFlags
        {
            CDDS_PREPAINT = 0x00000001,
            CDDS_POSTPAINT = 0x00000002,
            CDDS_PREERASE = 0x00000003,
            CDDS_POSTERASE = 0x00000004,
            CDDS_ITEM = 0x00010000,
            CDDS_ITEMPREPAINT = (CDDS_ITEM | CDDS_PREPAINT),
            CDDS_ITEMPOSTPAINT = (CDDS_ITEM | CDDS_POSTPAINT),
            CDDS_ITEMPREERASE = (CDDS_ITEM | CDDS_PREERASE),
            CDDS_ITEMPOSTERASE = (CDDS_ITEM | CDDS_POSTERASE),
            CDDS_SUBITEM = 0x00020000
        }

        // Custom Draw Return Flags
        public enum CustomDrawReturnFlags
        {
            CDRF_DODEFAULT = 0x00000000,
            CDRF_NEWFONT = 0x00000002,
            CDRF_SKIPDEFAULT = 0x00000004,
            CDRF_NOTIFYPOSTPAINT = 0x00000010,
            CDRF_NOTIFYITEMDRAW = 0x00000020,
            CDRF_NOTIFYSUBITEMDRAW = 0x00000020,
            CDRF_NOTIFYPOSTERASE = 0x00000040
        }


        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;

            public POINT()
            {
            }

            public POINT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
        public struct TV_ITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            public IntPtr /* LPTSTR */ pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHDR
        {
            public IntPtr hwndFrom;
            public int idFrom;
            public int code;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMTREEVIEW
        {
            public NMHDR nmhdr;
            public int action;
            public TV_ITEM itemOld;
            public TV_ITEM itemNew;
            public int ptDrag_X; // This should be declared as POINT
            public int ptDrag_Y; // we use unsafe blocks to manipulate 
            // NMTREEVIEW quickly, and POINT is declared
            // as a class.  Too much churn to change POINT
            // now.
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
        public class TV_HITTESTINFO
        {
            public int pt_x;
            public int pt_y;
            public int flags = 0;
            public IntPtr hItem = IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMCUSTOMDRAW
        {
            public NMHDR hdr;
            public int dwDrawStage;
            public IntPtr hdc;
            public RECT rc;
            public int dwItemSpec;
            public int uItemState;
            public int lItemlParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMTVCUSTOMDRAW
        {
            public NMCUSTOMDRAW nmcd;
            public uint clrText;
            public uint clrTextBk;
            public int iLevel;
        }


        // Tree View Hit Test Flag
        public enum TreeViewHitTestFlags
        {
            TVHT_NOWHERE = 0x0001,
            TVHT_ONITEMICON = 0x0002,
            TVHT_ONITEMLABEL = 0x0004,
            TVHT_ONITEMINDENT = 0x0008,
            TVHT_ONITEMBUTTON = 0x0010,
            TVHT_ONITEMRIGHT = 0x0020,
            TVHT_ONITEMSTATEICON = 0x0040,
            TVHT_ABOVE = 0x0100,
            TVHT_BELOW = 0x0200,
            TVHT_TORIGHT = 0x0400,
            TVHT_TOLEFT = 0x0800,
            TVHT_ONITEM = (0x0002 | 0x0004 | 0x0040)
        }

        public static IntPtr MAKELPARAM(int low, int high)
        {
            return (IntPtr)((high << 16) | (low & 0xffff));
        }

        const string SHELL32 = "shell32.dll";

        [DllImport(SHELL32, CharSet = CharSet.Auto)]
        public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        const string USER32 = "user32.dll";

        //TODO: Why isn't this exposed in the URT in a better fashion?
        [DllImport(USER32, ExactSpelling = true, CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        public static extern bool MessageBeep(/*int*/MB type);

        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, int lParam);

        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, TV_HITTESTINFO lParam);

        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out, MarshalAs(UnmanagedType.LPStruct)] Native.CHARFORMAT2A lParam);

        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, int wParam, [In, Out, MarshalAs(UnmanagedType.LPStruct)] Native.PARAFORMAT2 lParam);

        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, TV_HITTESTINFO lParam);

        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, ref TV_ITEM lParam);

        [DllImport(USER32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern short GetKeyState(int keyCode);

        [DllImport(USER32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetMessagePos();
    };
}


#endif
