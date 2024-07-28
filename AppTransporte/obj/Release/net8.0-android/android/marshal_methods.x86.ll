; ModuleID = 'marshal_methods.x86.ll'
source_filename = "marshal_methods.x86.ll"
target datalayout = "e-m:e-p:32:32-p270:32:32-p271:32:32-p272:64:64-f64:32:64-f80:32-n8:16:32-S128"
target triple = "i686-unknown-linux-android21"

%struct.MarshalMethodName = type {
	i64, ; uint64_t id
	ptr ; char* name
}

%struct.MarshalMethodsManagedClass = type {
	i32, ; uint32_t token
	ptr ; MonoClass klass
}

@assembly_image_cache = dso_local local_unnamed_addr global [123 x ptr] zeroinitializer, align 4

; Each entry maps hash of an assembly name to an index into the `assembly_image_cache` array
@assembly_image_cache_hashes = dso_local local_unnamed_addr constant [246 x i32] [
	i32 42639949, ; 0: System.Threading.Thread => 0x28aa24d => 114
	i32 67008169, ; 1: zh-Hant\Microsoft.Maui.Controls.resources => 0x3fe76a9 => 33
	i32 72070932, ; 2: Microsoft.Maui.Graphics.dll => 0x44bb714 => 47
	i32 117431740, ; 3: System.Runtime.InteropServices => 0x6ffddbc => 109
	i32 165246403, ; 4: Xamarin.AndroidX.Collection.dll => 0x9d975c3 => 60
	i32 182336117, ; 5: Xamarin.AndroidX.SwipeRefreshLayout.dll => 0xade3a75 => 78
	i32 195452805, ; 6: vi/Microsoft.Maui.Controls.resources.dll => 0xba65f85 => 30
	i32 199333315, ; 7: zh-HK/Microsoft.Maui.Controls.resources.dll => 0xbe195c3 => 31
	i32 205061960, ; 8: System.ComponentModel => 0xc38ff48 => 92
	i32 280992041, ; 9: cs/Microsoft.Maui.Controls.resources.dll => 0x10bf9929 => 2
	i32 317674968, ; 10: vi\Microsoft.Maui.Controls.resources => 0x12ef55d8 => 30
	i32 318968648, ; 11: Xamarin.AndroidX.Activity.dll => 0x13031348 => 56
	i32 336156722, ; 12: ja/Microsoft.Maui.Controls.resources.dll => 0x14095832 => 15
	i32 342366114, ; 13: Xamarin.AndroidX.Lifecycle.Common => 0x146817a2 => 67
	i32 356389973, ; 14: it/Microsoft.Maui.Controls.resources.dll => 0x153e1455 => 14
	i32 364942007, ; 15: SkiaSharp.Extended.UI => 0x15c092b7 => 50
	i32 379916513, ; 16: System.Threading.Thread.dll => 0x16a510e1 => 114
	i32 382590210, ; 17: SkiaSharp.Extended.dll => 0x16cddd02 => 49
	i32 385762202, ; 18: System.Memory.dll => 0x16fe439a => 101
	i32 395744057, ; 19: _Microsoft.Android.Resource.Designer => 0x17969339 => 34
	i32 435591531, ; 20: sv/Microsoft.Maui.Controls.resources.dll => 0x19f6996b => 26
	i32 442565967, ; 21: System.Collections => 0x1a61054f => 89
	i32 450948140, ; 22: Xamarin.AndroidX.Fragment.dll => 0x1ae0ec2c => 66
	i32 469710990, ; 23: System.dll => 0x1bff388e => 117
	i32 498788369, ; 24: System.ObjectModel => 0x1dbae811 => 106
	i32 500358224, ; 25: id/Microsoft.Maui.Controls.resources.dll => 0x1dd2dc50 => 13
	i32 503918385, ; 26: fi/Microsoft.Maui.Controls.resources.dll => 0x1e092f31 => 7
	i32 504833739, ; 27: SkiaSharp.SceneGraph => 0x1e1726cb => 51
	i32 513247710, ; 28: Microsoft.Extensions.Primitives.dll => 0x1e9789de => 42
	i32 525008092, ; 29: SkiaSharp.dll => 0x1f4afcdc => 48
	i32 539058512, ; 30: Microsoft.Extensions.Logging => 0x20216150 => 39
	i32 592146354, ; 31: pt-BR/Microsoft.Maui.Controls.resources.dll => 0x234b6fb2 => 21
	i32 627609679, ; 32: Xamarin.AndroidX.CustomView => 0x2568904f => 64
	i32 627931235, ; 33: nl\Microsoft.Maui.Controls.resources => 0x256d7863 => 19
	i32 672442732, ; 34: System.Collections.Concurrent => 0x2814a96c => 86
	i32 688181140, ; 35: ca/Microsoft.Maui.Controls.resources.dll => 0x2904cf94 => 1
	i32 690602616, ; 36: SkiaSharp.Skottie.dll => 0x2929c278 => 52
	i32 706645707, ; 37: ko/Microsoft.Maui.Controls.resources.dll => 0x2a1e8ecb => 16
	i32 709557578, ; 38: de/Microsoft.Maui.Controls.resources.dll => 0x2a4afd4a => 4
	i32 722857257, ; 39: System.Runtime.Loader.dll => 0x2b15ed29 => 110
	i32 738469988, ; 40: SkiaSharp.SceneGraph.dll => 0x2c042864 => 51
	i32 759454413, ; 41: System.Net.Requests => 0x2d445acd => 104
	i32 775507847, ; 42: System.IO.Compression => 0x2e394f87 => 98
	i32 777317022, ; 43: sk\Microsoft.Maui.Controls.resources => 0x2e54ea9e => 25
	i32 778804420, ; 44: SkiaSharp.Extended.UI.dll => 0x2e6b9cc4 => 50
	i32 789151979, ; 45: Microsoft.Extensions.Options => 0x2f0980eb => 41
	i32 797429506, ; 46: AppTransporte.dll => 0x2f87cf02 => 84
	i32 823281589, ; 47: System.Private.Uri.dll => 0x311247b5 => 107
	i32 830298997, ; 48: System.IO.Compression.Brotli => 0x317d5b75 => 97
	i32 904024072, ; 49: System.ComponentModel.Primitives.dll => 0x35e25008 => 90
	i32 926902833, ; 50: tr/Microsoft.Maui.Controls.resources.dll => 0x373f6a31 => 28
	i32 967690846, ; 51: Xamarin.AndroidX.Lifecycle.Common.dll => 0x39adca5e => 67
	i32 992768348, ; 52: System.Collections.dll => 0x3b2c715c => 89
	i32 1012816738, ; 53: Xamarin.AndroidX.SavedState.dll => 0x3c5e5b62 => 77
	i32 1019214401, ; 54: System.Drawing => 0x3cbffa41 => 96
	i32 1028951442, ; 55: Microsoft.Extensions.DependencyInjection.Abstractions => 0x3d548d92 => 38
	i32 1029334545, ; 56: da/Microsoft.Maui.Controls.resources.dll => 0x3d5a6611 => 3
	i32 1035644815, ; 57: Xamarin.AndroidX.AppCompat => 0x3dbaaf8f => 57
	i32 1036536393, ; 58: System.Drawing.Primitives.dll => 0x3dc84a49 => 95
	i32 1044663988, ; 59: System.Linq.Expressions.dll => 0x3e444eb4 => 99
	i32 1052210849, ; 60: Xamarin.AndroidX.Lifecycle.ViewModel.dll => 0x3eb776a1 => 69
	i32 1082857460, ; 61: System.ComponentModel.TypeConverter => 0x408b17f4 => 91
	i32 1084122840, ; 62: Xamarin.Kotlin.StdLib => 0x409e66d8 => 82
	i32 1098259244, ; 63: System => 0x41761b2c => 117
	i32 1118262833, ; 64: ko\Microsoft.Maui.Controls.resources => 0x42a75631 => 16
	i32 1168523401, ; 65: pt\Microsoft.Maui.Controls.resources => 0x45a64089 => 22
	i32 1178241025, ; 66: Xamarin.AndroidX.Navigation.Runtime.dll => 0x463a8801 => 74
	i32 1203215381, ; 67: pl/Microsoft.Maui.Controls.resources.dll => 0x47b79c15 => 20
	i32 1234928153, ; 68: nb/Microsoft.Maui.Controls.resources.dll => 0x499b8219 => 18
	i32 1260983243, ; 69: cs\Microsoft.Maui.Controls.resources => 0x4b2913cb => 2
	i32 1293217323, ; 70: Xamarin.AndroidX.DrawerLayout.dll => 0x4d14ee2b => 65
	i32 1324164729, ; 71: System.Linq => 0x4eed2679 => 100
	i32 1373134921, ; 72: zh-Hans\Microsoft.Maui.Controls.resources => 0x51d86049 => 32
	i32 1376866003, ; 73: Xamarin.AndroidX.SavedState => 0x52114ed3 => 77
	i32 1406073936, ; 74: Xamarin.AndroidX.CoordinatorLayout => 0x53cefc50 => 61
	i32 1430672901, ; 75: ar\Microsoft.Maui.Controls.resources => 0x55465605 => 0
	i32 1461004990, ; 76: es\Microsoft.Maui.Controls.resources => 0x57152abe => 6
	i32 1462112819, ; 77: System.IO.Compression.dll => 0x57261233 => 98
	i32 1469204771, ; 78: Xamarin.AndroidX.AppCompat.AppCompatResources => 0x57924923 => 58
	i32 1470490898, ; 79: Microsoft.Extensions.Primitives => 0x57a5e912 => 42
	i32 1480492111, ; 80: System.IO.Compression.Brotli.dll => 0x583e844f => 97
	i32 1493001747, ; 81: hi/Microsoft.Maui.Controls.resources.dll => 0x58fd6613 => 10
	i32 1514721132, ; 82: el/Microsoft.Maui.Controls.resources.dll => 0x5a48cf6c => 5
	i32 1543031311, ; 83: System.Text.RegularExpressions.dll => 0x5bf8ca0f => 113
	i32 1551623176, ; 84: sk/Microsoft.Maui.Controls.resources.dll => 0x5c7be408 => 25
	i32 1622152042, ; 85: Xamarin.AndroidX.Loader.dll => 0x60b0136a => 71
	i32 1623212457, ; 86: SkiaSharp.Views.Maui.Controls => 0x60c041a9 => 54
	i32 1624863272, ; 87: Xamarin.AndroidX.ViewPager2 => 0x60d97228 => 80
	i32 1636350590, ; 88: Xamarin.AndroidX.CursorAdapter => 0x6188ba7e => 63
	i32 1639515021, ; 89: System.Net.Http.dll => 0x61b9038d => 102
	i32 1639986890, ; 90: System.Text.RegularExpressions => 0x61c036ca => 113
	i32 1657153582, ; 91: System.Runtime => 0x62c6282e => 111
	i32 1658251792, ; 92: Xamarin.Google.Android.Material.dll => 0x62d6ea10 => 81
	i32 1677501392, ; 93: System.Net.Primitives.dll => 0x63fca3d0 => 103
	i32 1679769178, ; 94: System.Security.Cryptography => 0x641f3e5a => 112
	i32 1724472758, ; 95: SkiaSharp.Extended => 0x66c95db6 => 49
	i32 1729485958, ; 96: Xamarin.AndroidX.CardView.dll => 0x6715dc86 => 59
	i32 1736233607, ; 97: ro/Microsoft.Maui.Controls.resources.dll => 0x677cd287 => 23
	i32 1743415430, ; 98: ca\Microsoft.Maui.Controls.resources => 0x67ea6886 => 1
	i32 1766324549, ; 99: Xamarin.AndroidX.SwipeRefreshLayout => 0x6947f945 => 78
	i32 1770582343, ; 100: Microsoft.Extensions.Logging.dll => 0x6988f147 => 39
	i32 1780572499, ; 101: Mono.Android.Runtime.dll => 0x6a216153 => 121
	i32 1782862114, ; 102: ms\Microsoft.Maui.Controls.resources => 0x6a445122 => 17
	i32 1788241197, ; 103: Xamarin.AndroidX.Fragment => 0x6a96652d => 66
	i32 1793755602, ; 104: he\Microsoft.Maui.Controls.resources => 0x6aea89d2 => 9
	i32 1808609942, ; 105: Xamarin.AndroidX.Loader => 0x6bcd3296 => 71
	i32 1813058853, ; 106: Xamarin.Kotlin.StdLib.dll => 0x6c111525 => 82
	i32 1813201214, ; 107: Xamarin.Google.Android.Material => 0x6c13413e => 81
	i32 1818569960, ; 108: Xamarin.AndroidX.Navigation.UI.dll => 0x6c652ce8 => 75
	i32 1828688058, ; 109: Microsoft.Extensions.Logging.Abstractions.dll => 0x6cff90ba => 40
	i32 1842015223, ; 110: uk/Microsoft.Maui.Controls.resources.dll => 0x6dcaebf7 => 29
	i32 1853025655, ; 111: sv\Microsoft.Maui.Controls.resources => 0x6e72ed77 => 26
	i32 1858542181, ; 112: System.Linq.Expressions => 0x6ec71a65 => 99
	i32 1875935024, ; 113: fr\Microsoft.Maui.Controls.resources => 0x6fd07f30 => 8
	i32 1910275211, ; 114: System.Collections.NonGeneric.dll => 0x71dc7c8b => 87
	i32 1968388702, ; 115: Microsoft.Extensions.Configuration.dll => 0x75533a5e => 35
	i32 2003115576, ; 116: el\Microsoft.Maui.Controls.resources => 0x77651e38 => 5
	i32 2011961780, ; 117: System.Buffers.dll => 0x77ec19b4 => 85
	i32 2019465201, ; 118: Xamarin.AndroidX.Lifecycle.ViewModel => 0x785e97f1 => 69
	i32 2025202353, ; 119: ar/Microsoft.Maui.Controls.resources.dll => 0x78b622b1 => 0
	i32 2045470958, ; 120: System.Private.Xml => 0x79eb68ee => 108
	i32 2055257422, ; 121: Xamarin.AndroidX.Lifecycle.LiveData.Core.dll => 0x7a80bd4e => 68
	i32 2066184531, ; 122: de\Microsoft.Maui.Controls.resources => 0x7b277953 => 4
	i32 2079903147, ; 123: System.Runtime.dll => 0x7bf8cdab => 111
	i32 2090596640, ; 124: System.Numerics.Vectors => 0x7c9bf920 => 105
	i32 2127167465, ; 125: System.Console => 0x7ec9ffe9 => 93
	i32 2142473426, ; 126: System.Collections.Specialized => 0x7fb38cd2 => 88
	i32 2159891885, ; 127: Microsoft.Maui => 0x80bd55ad => 45
	i32 2169148018, ; 128: hu\Microsoft.Maui.Controls.resources => 0x814a9272 => 12
	i32 2181898931, ; 129: Microsoft.Extensions.Options.dll => 0x820d22b3 => 41
	i32 2192057212, ; 130: Microsoft.Extensions.Logging.Abstractions => 0x82a8237c => 40
	i32 2193016926, ; 131: System.ObjectModel.dll => 0x82b6c85e => 106
	i32 2201107256, ; 132: Xamarin.KotlinX.Coroutines.Core.Jvm.dll => 0x83323b38 => 83
	i32 2201231467, ; 133: System.Net.Http => 0x8334206b => 102
	i32 2207618523, ; 134: it\Microsoft.Maui.Controls.resources => 0x839595db => 14
	i32 2266799131, ; 135: Microsoft.Extensions.Configuration.Abstractions => 0x871c9c1b => 36
	i32 2270573516, ; 136: fr/Microsoft.Maui.Controls.resources.dll => 0x875633cc => 8
	i32 2279755925, ; 137: Xamarin.AndroidX.RecyclerView.dll => 0x87e25095 => 76
	i32 2303942373, ; 138: nb\Microsoft.Maui.Controls.resources => 0x89535ee5 => 18
	i32 2305521784, ; 139: System.Private.CoreLib.dll => 0x896b7878 => 119
	i32 2353062107, ; 140: System.Net.Primitives => 0x8c40e0db => 103
	i32 2364201794, ; 141: SkiaSharp.Views.Maui.Core => 0x8ceadb42 => 55
	i32 2368005991, ; 142: System.Xml.ReaderWriter.dll => 0x8d24e767 => 116
	i32 2371007202, ; 143: Microsoft.Extensions.Configuration => 0x8d52b2e2 => 35
	i32 2395872292, ; 144: id\Microsoft.Maui.Controls.resources => 0x8ece1c24 => 13
	i32 2427813419, ; 145: hi\Microsoft.Maui.Controls.resources => 0x90b57e2b => 10
	i32 2435356389, ; 146: System.Console.dll => 0x912896e5 => 93
	i32 2471841756, ; 147: netstandard.dll => 0x93554fdc => 118
	i32 2475788418, ; 148: Java.Interop.dll => 0x93918882 => 120
	i32 2480646305, ; 149: Microsoft.Maui.Controls => 0x93dba8a1 => 43
	i32 2550873716, ; 150: hr\Microsoft.Maui.Controls.resources => 0x980b3e74 => 11
	i32 2593496499, ; 151: pl\Microsoft.Maui.Controls.resources => 0x9a959db3 => 20
	i32 2605712449, ; 152: Xamarin.KotlinX.Coroutines.Core.Jvm => 0x9b500441 => 83
	i32 2617129537, ; 153: System.Private.Xml.dll => 0x9bfe3a41 => 108
	i32 2620871830, ; 154: Xamarin.AndroidX.CursorAdapter.dll => 0x9c375496 => 63
	i32 2625339995, ; 155: SkiaSharp.Views.Maui.Core.dll => 0x9c7b825b => 55
	i32 2626831493, ; 156: ja\Microsoft.Maui.Controls.resources => 0x9c924485 => 15
	i32 2663698177, ; 157: System.Runtime.Loader => 0x9ec4cf01 => 110
	i32 2665622720, ; 158: System.Drawing.Primitives => 0x9ee22cc0 => 95
	i32 2732626843, ; 159: Xamarin.AndroidX.Activity => 0xa2e0939b => 56
	i32 2737747696, ; 160: Xamarin.AndroidX.AppCompat.AppCompatResources.dll => 0xa32eb6f0 => 58
	i32 2752995522, ; 161: pt-BR\Microsoft.Maui.Controls.resources => 0xa41760c2 => 21
	i32 2758225723, ; 162: Microsoft.Maui.Controls.Xaml => 0xa4672f3b => 44
	i32 2764765095, ; 163: Microsoft.Maui.dll => 0xa4caf7a7 => 45
	i32 2778768386, ; 164: Xamarin.AndroidX.ViewPager.dll => 0xa5a0a402 => 79
	i32 2785988530, ; 165: th\Microsoft.Maui.Controls.resources => 0xa60ecfb2 => 27
	i32 2795602088, ; 166: SkiaSharp.Views.Android.dll => 0xa6a180a8 => 53
	i32 2801831435, ; 167: Microsoft.Maui.Graphics => 0xa7008e0b => 47
	i32 2806116107, ; 168: es/Microsoft.Maui.Controls.resources.dll => 0xa741ef0b => 6
	i32 2810250172, ; 169: Xamarin.AndroidX.CoordinatorLayout.dll => 0xa78103bc => 61
	i32 2831556043, ; 170: nl/Microsoft.Maui.Controls.resources.dll => 0xa8c61dcb => 19
	i32 2853208004, ; 171: Xamarin.AndroidX.ViewPager => 0xaa107fc4 => 79
	i32 2861189240, ; 172: Microsoft.Maui.Essentials => 0xaa8a4878 => 46
	i32 2909740682, ; 173: System.Private.CoreLib => 0xad6f1e8a => 119
	i32 2912489636, ; 174: SkiaSharp.Views.Android => 0xad9910a4 => 53
	i32 2916838712, ; 175: Xamarin.AndroidX.ViewPager2.dll => 0xaddb6d38 => 80
	i32 2919462931, ; 176: System.Numerics.Vectors.dll => 0xae037813 => 105
	i32 2959614098, ; 177: System.ComponentModel.dll => 0xb0682092 => 92
	i32 2978675010, ; 178: Xamarin.AndroidX.DrawerLayout => 0xb18af942 => 65
	i32 3038032645, ; 179: _Microsoft.Android.Resource.Designer.dll => 0xb514b305 => 34
	i32 3057625584, ; 180: Xamarin.AndroidX.Navigation.Common => 0xb63fa9f0 => 72
	i32 3059408633, ; 181: Mono.Android.Runtime => 0xb65adef9 => 121
	i32 3059793426, ; 182: System.ComponentModel.Primitives => 0xb660be12 => 90
	i32 3077302341, ; 183: hu/Microsoft.Maui.Controls.resources.dll => 0xb76be845 => 12
	i32 3178803400, ; 184: Xamarin.AndroidX.Navigation.Fragment.dll => 0xbd78b0c8 => 73
	i32 3220365878, ; 185: System.Threading => 0xbff2e236 => 115
	i32 3258312781, ; 186: Xamarin.AndroidX.CardView => 0xc235e84d => 59
	i32 3305363605, ; 187: fi\Microsoft.Maui.Controls.resources => 0xc503d895 => 7
	i32 3316684772, ; 188: System.Net.Requests.dll => 0xc5b097e4 => 104
	i32 3317135071, ; 189: Xamarin.AndroidX.CustomView.dll => 0xc5b776df => 64
	i32 3340387945, ; 190: SkiaSharp => 0xc71a4669 => 48
	i32 3346324047, ; 191: Xamarin.AndroidX.Navigation.Runtime => 0xc774da4f => 74
	i32 3357674450, ; 192: ru\Microsoft.Maui.Controls.resources => 0xc8220bd2 => 24
	i32 3362522851, ; 193: Xamarin.AndroidX.Core => 0xc86c06e3 => 62
	i32 3366347497, ; 194: Java.Interop => 0xc8a662e9 => 120
	i32 3374999561, ; 195: Xamarin.AndroidX.RecyclerView => 0xc92a6809 => 76
	i32 3381016424, ; 196: da\Microsoft.Maui.Controls.resources => 0xc9863768 => 3
	i32 3428513518, ; 197: Microsoft.Extensions.DependencyInjection.dll => 0xcc5af6ee => 37
	i32 3430777524, ; 198: netstandard => 0xcc7d82b4 => 118
	i32 3463511458, ; 199: hr/Microsoft.Maui.Controls.resources.dll => 0xce70fda2 => 11
	i32 3471940407, ; 200: System.ComponentModel.TypeConverter.dll => 0xcef19b37 => 91
	i32 3473156932, ; 201: SkiaSharp.Views.Maui.Controls.dll => 0xcf042b44 => 54
	i32 3476120550, ; 202: Mono.Android => 0xcf3163e6 => 122
	i32 3479583265, ; 203: ru/Microsoft.Maui.Controls.resources.dll => 0xcf663a21 => 24
	i32 3484440000, ; 204: ro\Microsoft.Maui.Controls.resources => 0xcfb055c0 => 23
	i32 3580758918, ; 205: zh-HK\Microsoft.Maui.Controls.resources => 0xd56e0b86 => 31
	i32 3608519521, ; 206: System.Linq.dll => 0xd715a361 => 100
	i32 3641597786, ; 207: Xamarin.AndroidX.Lifecycle.LiveData.Core => 0xd90e5f5a => 68
	i32 3643446276, ; 208: tr\Microsoft.Maui.Controls.resources => 0xd92a9404 => 28
	i32 3643854240, ; 209: Xamarin.AndroidX.Navigation.Fragment => 0xd930cda0 => 73
	i32 3657292374, ; 210: Microsoft.Extensions.Configuration.Abstractions.dll => 0xd9fdda56 => 36
	i32 3663323240, ; 211: SkiaSharp.Skottie => 0xda59e068 => 52
	i32 3672681054, ; 212: Mono.Android.dll => 0xdae8aa5e => 122
	i32 3697841164, ; 213: zh-Hant/Microsoft.Maui.Controls.resources.dll => 0xdc68940c => 33
	i32 3724971120, ; 214: Xamarin.AndroidX.Navigation.Common.dll => 0xde068c70 => 72
	i32 3748608112, ; 215: System.Diagnostics.DiagnosticSource => 0xdf6f3870 => 94
	i32 3786282454, ; 216: Xamarin.AndroidX.Collection => 0xe1ae15d6 => 60
	i32 3792276235, ; 217: System.Collections.NonGeneric => 0xe2098b0b => 87
	i32 3802395368, ; 218: System.Collections.Specialized.dll => 0xe2a3f2e8 => 88
	i32 3823053215, ; 219: AppTransporte => 0xe3df299f => 84
	i32 3823082795, ; 220: System.Security.Cryptography.dll => 0xe3df9d2b => 112
	i32 3841636137, ; 221: Microsoft.Extensions.DependencyInjection.Abstractions.dll => 0xe4fab729 => 38
	i32 3849253459, ; 222: System.Runtime.InteropServices.dll => 0xe56ef253 => 109
	i32 3889960447, ; 223: zh-Hans/Microsoft.Maui.Controls.resources.dll => 0xe7dc15ff => 32
	i32 3896106733, ; 224: System.Collections.Concurrent.dll => 0xe839deed => 86
	i32 3896760992, ; 225: Xamarin.AndroidX.Core.dll => 0xe843daa0 => 62
	i32 3928044579, ; 226: System.Xml.ReaderWriter => 0xea213423 => 116
	i32 3931092270, ; 227: Xamarin.AndroidX.Navigation.UI => 0xea4fb52e => 75
	i32 3955647286, ; 228: Xamarin.AndroidX.AppCompat.dll => 0xebc66336 => 57
	i32 3980434154, ; 229: th/Microsoft.Maui.Controls.resources.dll => 0xed409aea => 27
	i32 3987592930, ; 230: he/Microsoft.Maui.Controls.resources.dll => 0xedadd6e2 => 9
	i32 4025784931, ; 231: System.Memory => 0xeff49a63 => 101
	i32 4046471985, ; 232: Microsoft.Maui.Controls.Xaml.dll => 0xf1304331 => 44
	i32 4073602200, ; 233: System.Threading.dll => 0xf2ce3c98 => 115
	i32 4094352644, ; 234: Microsoft.Maui.Essentials.dll => 0xf40add04 => 46
	i32 4099507663, ; 235: System.Drawing.dll => 0xf45985cf => 96
	i32 4100113165, ; 236: System.Private.Uri => 0xf462c30d => 107
	i32 4102112229, ; 237: pt/Microsoft.Maui.Controls.resources.dll => 0xf48143e5 => 22
	i32 4125707920, ; 238: ms/Microsoft.Maui.Controls.resources.dll => 0xf5e94e90 => 17
	i32 4126470640, ; 239: Microsoft.Extensions.DependencyInjection => 0xf5f4f1f0 => 37
	i32 4150914736, ; 240: uk\Microsoft.Maui.Controls.resources => 0xf769eeb0 => 29
	i32 4182413190, ; 241: Xamarin.AndroidX.Lifecycle.ViewModelSavedState.dll => 0xf94a8f86 => 70
	i32 4213026141, ; 242: System.Diagnostics.DiagnosticSource.dll => 0xfb1dad5d => 94
	i32 4260525087, ; 243: System.Buffers => 0xfdf2741f => 85
	i32 4271975918, ; 244: Microsoft.Maui.Controls.dll => 0xfea12dee => 43
	i32 4292120959 ; 245: Xamarin.AndroidX.Lifecycle.ViewModelSavedState => 0xffd4917f => 70
], align 4

@assembly_image_cache_indices = dso_local local_unnamed_addr constant [246 x i32] [
	i32 114, ; 0
	i32 33, ; 1
	i32 47, ; 2
	i32 109, ; 3
	i32 60, ; 4
	i32 78, ; 5
	i32 30, ; 6
	i32 31, ; 7
	i32 92, ; 8
	i32 2, ; 9
	i32 30, ; 10
	i32 56, ; 11
	i32 15, ; 12
	i32 67, ; 13
	i32 14, ; 14
	i32 50, ; 15
	i32 114, ; 16
	i32 49, ; 17
	i32 101, ; 18
	i32 34, ; 19
	i32 26, ; 20
	i32 89, ; 21
	i32 66, ; 22
	i32 117, ; 23
	i32 106, ; 24
	i32 13, ; 25
	i32 7, ; 26
	i32 51, ; 27
	i32 42, ; 28
	i32 48, ; 29
	i32 39, ; 30
	i32 21, ; 31
	i32 64, ; 32
	i32 19, ; 33
	i32 86, ; 34
	i32 1, ; 35
	i32 52, ; 36
	i32 16, ; 37
	i32 4, ; 38
	i32 110, ; 39
	i32 51, ; 40
	i32 104, ; 41
	i32 98, ; 42
	i32 25, ; 43
	i32 50, ; 44
	i32 41, ; 45
	i32 84, ; 46
	i32 107, ; 47
	i32 97, ; 48
	i32 90, ; 49
	i32 28, ; 50
	i32 67, ; 51
	i32 89, ; 52
	i32 77, ; 53
	i32 96, ; 54
	i32 38, ; 55
	i32 3, ; 56
	i32 57, ; 57
	i32 95, ; 58
	i32 99, ; 59
	i32 69, ; 60
	i32 91, ; 61
	i32 82, ; 62
	i32 117, ; 63
	i32 16, ; 64
	i32 22, ; 65
	i32 74, ; 66
	i32 20, ; 67
	i32 18, ; 68
	i32 2, ; 69
	i32 65, ; 70
	i32 100, ; 71
	i32 32, ; 72
	i32 77, ; 73
	i32 61, ; 74
	i32 0, ; 75
	i32 6, ; 76
	i32 98, ; 77
	i32 58, ; 78
	i32 42, ; 79
	i32 97, ; 80
	i32 10, ; 81
	i32 5, ; 82
	i32 113, ; 83
	i32 25, ; 84
	i32 71, ; 85
	i32 54, ; 86
	i32 80, ; 87
	i32 63, ; 88
	i32 102, ; 89
	i32 113, ; 90
	i32 111, ; 91
	i32 81, ; 92
	i32 103, ; 93
	i32 112, ; 94
	i32 49, ; 95
	i32 59, ; 96
	i32 23, ; 97
	i32 1, ; 98
	i32 78, ; 99
	i32 39, ; 100
	i32 121, ; 101
	i32 17, ; 102
	i32 66, ; 103
	i32 9, ; 104
	i32 71, ; 105
	i32 82, ; 106
	i32 81, ; 107
	i32 75, ; 108
	i32 40, ; 109
	i32 29, ; 110
	i32 26, ; 111
	i32 99, ; 112
	i32 8, ; 113
	i32 87, ; 114
	i32 35, ; 115
	i32 5, ; 116
	i32 85, ; 117
	i32 69, ; 118
	i32 0, ; 119
	i32 108, ; 120
	i32 68, ; 121
	i32 4, ; 122
	i32 111, ; 123
	i32 105, ; 124
	i32 93, ; 125
	i32 88, ; 126
	i32 45, ; 127
	i32 12, ; 128
	i32 41, ; 129
	i32 40, ; 130
	i32 106, ; 131
	i32 83, ; 132
	i32 102, ; 133
	i32 14, ; 134
	i32 36, ; 135
	i32 8, ; 136
	i32 76, ; 137
	i32 18, ; 138
	i32 119, ; 139
	i32 103, ; 140
	i32 55, ; 141
	i32 116, ; 142
	i32 35, ; 143
	i32 13, ; 144
	i32 10, ; 145
	i32 93, ; 146
	i32 118, ; 147
	i32 120, ; 148
	i32 43, ; 149
	i32 11, ; 150
	i32 20, ; 151
	i32 83, ; 152
	i32 108, ; 153
	i32 63, ; 154
	i32 55, ; 155
	i32 15, ; 156
	i32 110, ; 157
	i32 95, ; 158
	i32 56, ; 159
	i32 58, ; 160
	i32 21, ; 161
	i32 44, ; 162
	i32 45, ; 163
	i32 79, ; 164
	i32 27, ; 165
	i32 53, ; 166
	i32 47, ; 167
	i32 6, ; 168
	i32 61, ; 169
	i32 19, ; 170
	i32 79, ; 171
	i32 46, ; 172
	i32 119, ; 173
	i32 53, ; 174
	i32 80, ; 175
	i32 105, ; 176
	i32 92, ; 177
	i32 65, ; 178
	i32 34, ; 179
	i32 72, ; 180
	i32 121, ; 181
	i32 90, ; 182
	i32 12, ; 183
	i32 73, ; 184
	i32 115, ; 185
	i32 59, ; 186
	i32 7, ; 187
	i32 104, ; 188
	i32 64, ; 189
	i32 48, ; 190
	i32 74, ; 191
	i32 24, ; 192
	i32 62, ; 193
	i32 120, ; 194
	i32 76, ; 195
	i32 3, ; 196
	i32 37, ; 197
	i32 118, ; 198
	i32 11, ; 199
	i32 91, ; 200
	i32 54, ; 201
	i32 122, ; 202
	i32 24, ; 203
	i32 23, ; 204
	i32 31, ; 205
	i32 100, ; 206
	i32 68, ; 207
	i32 28, ; 208
	i32 73, ; 209
	i32 36, ; 210
	i32 52, ; 211
	i32 122, ; 212
	i32 33, ; 213
	i32 72, ; 214
	i32 94, ; 215
	i32 60, ; 216
	i32 87, ; 217
	i32 88, ; 218
	i32 84, ; 219
	i32 112, ; 220
	i32 38, ; 221
	i32 109, ; 222
	i32 32, ; 223
	i32 86, ; 224
	i32 62, ; 225
	i32 116, ; 226
	i32 75, ; 227
	i32 57, ; 228
	i32 27, ; 229
	i32 9, ; 230
	i32 101, ; 231
	i32 44, ; 232
	i32 115, ; 233
	i32 46, ; 234
	i32 96, ; 235
	i32 107, ; 236
	i32 22, ; 237
	i32 17, ; 238
	i32 37, ; 239
	i32 29, ; 240
	i32 70, ; 241
	i32 94, ; 242
	i32 85, ; 243
	i32 43, ; 244
	i32 70 ; 245
], align 4

@marshal_methods_number_of_classes = dso_local local_unnamed_addr constant i32 0, align 4

@marshal_methods_class_cache = dso_local local_unnamed_addr global [0 x %struct.MarshalMethodsManagedClass] zeroinitializer, align 4

; Names of classes in which marshal methods reside
@mm_class_names = dso_local local_unnamed_addr constant [0 x ptr] zeroinitializer, align 4

@mm_method_names = dso_local local_unnamed_addr constant [1 x %struct.MarshalMethodName] [
	%struct.MarshalMethodName {
		i64 0, ; id 0x0; name: 
		ptr @.MarshalMethodName.0_name; char* name
	} ; 0
], align 8

; get_function_pointer (uint32_t mono_image_index, uint32_t class_index, uint32_t method_token, void*& target_ptr)
@get_function_pointer = internal dso_local unnamed_addr global ptr null, align 4

; Functions

; Function attributes: "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" uwtable willreturn
define void @xamarin_app_init(ptr nocapture noundef readnone %env, ptr noundef %fn) local_unnamed_addr #0
{
	%fnIsNull = icmp eq ptr %fn, null
	br i1 %fnIsNull, label %1, label %2

1: ; preds = %0
	%putsResult = call noundef i32 @puts(ptr @.str.0)
	call void @abort()
	unreachable 

2: ; preds = %1, %0
	store ptr %fn, ptr @get_function_pointer, align 4, !tbaa !3
	ret void
}

; Strings
@.str.0 = private unnamed_addr constant [40 x i8] c"get_function_pointer MUST be specified\0A\00", align 1

;MarshalMethodName
@.MarshalMethodName.0_name = private unnamed_addr constant [1 x i8] c"\00", align 1

; External functions

; Function attributes: noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8"
declare void @abort() local_unnamed_addr #2

; Function attributes: nofree nounwind
declare noundef i32 @puts(ptr noundef) local_unnamed_addr #1
attributes #0 = { "min-legal-vector-width"="0" mustprogress nofree norecurse nosync "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" uwtable willreturn }
attributes #1 = { nofree nounwind }
attributes #2 = { noreturn "no-trapping-math"="true" nounwind "stack-protector-buffer-size"="8" "stackrealign" "target-cpu"="i686" "target-features"="+cx8,+mmx,+sse,+sse2,+sse3,+ssse3,+x87" "tune-cpu"="generic" }

; Metadata
!llvm.module.flags = !{!0, !1, !7}
!0 = !{i32 1, !"wchar_size", i32 4}
!1 = !{i32 7, !"PIC Level", i32 2}
!llvm.ident = !{!2}
!2 = !{!"Xamarin.Android remotes/origin/release/8.0.2xx @ 0d97e20b84d8e87c3502469ee395805907905fe3"}
!3 = !{!4, !4, i64 0}
!4 = !{!"any pointer", !5, i64 0}
!5 = !{!"omnipotent char", !6, i64 0}
!6 = !{!"Simple C++ TBAA"}
!7 = !{i32 1, !"NumRegisterParameters", i32 0}
