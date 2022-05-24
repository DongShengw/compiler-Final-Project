## 说明

项目文件 *.fsproj指定了 fslex 工具的调用参数
- 输入文件是  lex.fsl 
- 生成文件默认是 lex.fs
  - 生成文件模块名是 lex
```sh
<FsLex Include="lex.fsl">
<OtherFlags>--module lex --unicode</OtherFlags>
</FsLex
```
## 交互过程

```sh
dotnet build
compiling to dfas (can take a while...)
9 states
writing output  // 从词法说明 lex.fsl 生成了扫描器 lex.fs

dotnet fsi            

>#r "nuget: fslexyacc"  //加载依赖包
>#load "lex.fs"        //加载文件
>open lex             //导入模块
>main newlexbuf       //运行词法分析函数

。。。

Ready to lex.         //输出
namespace FSI_0003
  val trans : uint16 [] array
  val actions : uint16 []
  val _fslex_tables : FSharp.Text.Lexing.UnicodeTables
  val _fslex_dummy : unit -> 'a
  val main : lexbuf:FSharp.Text.Lexing.LexBuffer<char> -> unit
  val stdin : System.IO.Stream
  val newlexbuf : FSharp.Text.Lexing.LexBuffer<char>

3 3.5 "a"             //输入
Int
val it : unit = ()

> main newlexbuf;;  //取下个 token
Float
val it : unit = ()

> main newlexbuf;;
String
val it : unit = ()

> main newlexbuf;;       //lexbuf 空，继续等待输入
aaa
String
```
