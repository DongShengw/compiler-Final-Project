## 文件说明

### interpreter  解释器

```sh
Absyn.fs micro-C abstract syntax                              抽象语法
grammar.txt informal micro-C grammar and parser specification 文法定义
CLex.fsl micro-C lexer specification                          fslex词法定义
CPar.fsy micro-C parser specification                         fsyacc语法定义
Parse.fs micro-C parser                                       语法解析器
Interp.fs micro-C interpreter                                 解释器
example/ex1.c-ex25.c micro-C example programs                 例子程序
interpc.fsproj                                                项目文件

```

### compiler  编译器

```sh
StackMachine.fs definition of micro-C stack machine instructions  VM 指令定义
Machine.java micro-C stack machine in Java                   VM 实现 java
machine.c micro-C stack machine in C                         VM 实现 c 
machine.cs micro-C stack machine in CSharp                   VM 实现 c#
machine.csproj  machine project file                         VM 项目文件

Comp.fs compile micro-C to stack machine code             编译器 输出 stack vm 指令序列
Backend.fs x86_64 backend                                 编译器后端 翻译 stack vm 指令序列到 x86_64
driver.c     runtime support                                 运行时支持程序
prog0 example stack machine program: print numbers           字节码 案例，输出数字
prog1 example stack machine program: loop 20m times          字节码 案例，循环2千万次
microc.fsproj                                                编译器项目文件
```

### continuation compiler  优化编译器

```sh
Contcomp.fs compile micro-C backwards                   优化编译器
microcc.fsproj                                          优化编译器项目文件
```

## 构建与执行

### A 解释器

#### A.1  解释器 interpc.exe 构建

```sh
# 编译解释器 interpc.exe 命令行程序 
dotnet restore  interpc.fsproj   # 可选
dotnet clean  interpc.fsproj     # 可选
dotnet build -v n interpc.fsproj # 构建./bin/Debug/net6.0/interpc.exe ，-v n查看详细生成过程

# 执行解释器
./bin/Debug/net6.0/interpc.exe example/ex1.c 8
dotnet run --project interpc.fsproj example/ex1.c 8
dotnet run --project interpc.fsproj -g example/ex1.c 8  # 显示token AST 等调试信息

# one-liner 
# 自行修改 interpc.fsproj  解释example目录下的源文件
# 
# <MyItem Include="example\function1.c" Args ="8"/> 

dotnet build -t:ccrun interpc.fsproj
```

#### A.2 dotnet命令行fsi中运行解释器

```sh
# 生成扫描器
dotnet "C:\Users\gm\.nuget\packages\fslexyacc\10.2.0\build\/fslex/netcoreapp3.1\fslex.dll"  -o "CLex.fs" --module CLex --unicode CLex.fsl

# 生成分析器
dotnet "C:\Users\gm\.nuget\packages\fslexyacc\10.2.0\build\/fsyacc/netcoreapp3.1\fsyacc.dll"  -o "CPar.fs" --module CPar CPar.fsy

# 命令行运行程序
dotnet fsi 

#r "nuget: FsLexYacc";;  //添加包引用
#load "Absyn.fs" "Debug.fs" "CPar.fs" "CLex.fs" "Parse.fs" "Interp.fs" "ParseAndRun.fs" ;; 

open ParseAndRun;;    //导入模块 ParseAndRun
fromFile "example\ex1.c";;    //显示 ex1.c的语法树
run (fromFile "example\ex1.c") [17];; //解释执行 ex1.c
run (fromFile "example\ex11.c") [8];; //解释执行 ex11.c

Debug.debug <-  true  //打开调试

run (fromFile "example\ex1.c") [8];; //解释执行 ex1.c
run (fromFile "example\ex11.c") [8];; //解释执行 ex11.
#q;;

```

解释器的主入口 是 interp.fs 中的 run 函数，具体看代码的注释

### B 编译器

编译器生成 `example` 目录下的栈式虚拟机 `*.out` 文件，
`*.out` 文件 用 `步骤 D` 中的虚拟机执行。

#### B.1 microc编译器构建步骤

```sh
# 构建 microc.exe 编译器程序 
dotnet restore  microc.fsproj # 可选
dotnet clean  microc.fsproj   # 可选
dotnet build  microc.fsproj   # 构建 ./bin/Debug/net6.0/microc.exe

dotnet run --project microc.fsproj example/ex1.c    # 执行编译器，编译 ex1.c，并输出  ex1.out 文件
dotnet run --project microc.fsproj -g example/ex1.c   # -g 查看调试信息

./bin/Debug/net6.0/microc.exe -g example/ex1.c  # 直接执行构建的.exe文件，同上效果


```

#### B.2 dotnet fsi 中运行编译器

```sh
# 启动fsi
dotnet fsi

#r "nuget: FsLexYacc";;

#load "Absyn.fs"  "CPar.fs" "CLex.fs" "Debug.fs" "Parse.fs" "Machine.fs" "Backend.fs" "Comp.fs" "ParseAndComp.fs";;   

# 运行编译器
open ParseAndComp;;
compileToFile (fromFile "example\ex1.c") "ex1";; 

Debug.debug <-  true   # 打开调试
compileToFile (fromFile "example\ex4.c") "ex4";; # 观察变量在环境上的分配
#q;;


# fsi 中运行
#time "on";;  // 打开时间跟踪

# 参考A. 中的命令 比较下解释执行解释执行 与 编译执行 ex11.c 的速度
```

#### B.3 编译并运行 example 目录下多个文件

- 用到了 build  -t 任务 选项

- 运行编译器生成的 *.out  文件 需要先完成 D.2 ，在当前目录生成虚拟机`machine.exe`

```sh
dotnet build -t:cclean microc.fsproj    # 清除编译器生成的文件  example/*.ins *.out
dotnet build -t:ccrun microc.fsproj     # 编译并运行 example 目录下多个文件 

```


### C 优化编译器

#### C.1  优化编译器 microcc.exe 构建步骤

```sh

dotnet restore  microcc.fsproj
dotnet clean  microcc.fsproj
dotnet build  microcc.fsproj           # 构建编译器

dotnet run --project microcc.fsproj ex11.c    # 执行编译器
./bin/Debug/net6.0/microcc.exe ex11.c  # 直接执行

```

#### C.2 dotnet fsi 中运行 backwards编译器  

```sh
dotnet fsi 

#r "nuget: FsLexYacc";;

#load "Absyn.fs"  "CPar.fs" "CLex.fs" "Debug.fs" "Parse.fs" "Machine.fs" "Backend.fs" "Contcomp.fs" "ParseAndComp.fs";;   

open ParseAndContcomp;;
contCompileToFile (fromFile "example\ex11.c") "ex11.out";;
#q;;
```

### D 虚拟机构建与运行
虚拟机有 `c#` `c` `java` 三个版本
- 运行下面的命令 查看 fac 0 , fac 3 的栈帧
- 理解栈式虚拟机执行流程

执行前，先在B中 编译出 *.out 虚拟机指令文件

#### D.1 c#

```sh
dotnet clean machine.csproj
dotnet build machine.csproj   #构建虚拟机 machine.exe 

./bin/Debug/net6.0/machine.exe ./example/ex9.out 3  # 运行虚拟机，执行 ex9.out 
./bin/Debug/net6.0/machine.exe -t ./example/ex9.out 0  # 运行虚拟机，执行 ex9.out ，-t 查看跟踪信息
./bin/Debug/net6.0/machine.exe -t ./example/ex9.out 3  // 运行虚拟机，执行 ex9.out ，-t 查看跟踪信息
```

#### D.2 C

```sh
# 编译 c 虚拟机
gcc -o machine.exe machine.c

# 虚拟机执行指令
machine.exe ./example/ex9.out 3

# 调试执行指令
machine.exe -trace ./example/ex9.out 0  # -trace  并查看跟踪信息
machine.exe -trace ./example/ex9.out 3

```

#### D.3 Java

```sh
javac Machine.java
java Machine ./example/ex9.out 3

javac Machinetrace.java
java Machinetrace ./example/ex9.out 0
java Machinetrace ./example/ex9.out 3
```

#### E 编译到x86_64

#### 预备软件
nasm, gcc

```sh
#Linux
$sudo apt-get install build-essential nasm gcc

# Windows

# nasm 汇编器
https://www.nasm.us/pub/nasm/releasebuilds/2.15.05/win64/

# gcc 编译器
https://jmeubank.github.io/tdm-gcc/download/

w10 在 9.2.0 版本测试通过

```

#### 步骤

栈式虚拟机指令编译到x86_64，简单示例

分步构建

```sh

# 生成 ex1.asm 汇编码 nasm 格式
dotnet run --project microc.fsproj example/ex1.c

# 汇编生成目标文件
nasm -f win64 example/ex1.asm -o example/ex1.o   # win
# nasm -f elf64 ex1.asm -o ex1.o   # linux  

# 编译运行时文件
gcc -c driver.c

# 链接运行时，生成可执行程序
gcc -g -o example/ex1.exe driver.o example/ex1.o

# 执行
example/ex1.exe 8 

```

单步构建

```sh
# 使用 build target 编译 ex1.c
# 可修改 microc.fsproj 编译其他案例文件

dotnet build -t:ccrunx86 microc.fsproj

```

#### 调用约定

- caller
  - 调用函数前，在栈上放置函数参数，个数为m
  - 将rbp入栈，调用函数
- callee
  - 保存返回地址r，老的栈帧指针bp
  - 将参数搬移到本函数的栈帧初始位置
  - 将rbp设置到第一个参数的位置
  - rbx 保存函数的返回值

#### 数据在栈上的保存方式

- 如数组 a[2] 的元素按次序排在栈上，末尾保存数组变量a，内容是首元素 e0的地址
- e0, e1, a  

访问数组，先通过`BP`得到`a`的位置，然后通过`a` 得到首地址 e0，最后计算数组下标偏移地址，访问对应数组元素

- 全局变量在栈上依次保存，x86 汇编中，glovar 是全局变量的起始地址

#### x86 bugs

- *(p + 2) 指针运算不支持

#### tasks.json

默认任务`build & run`
- Ctrl+Shift+B

# Cuby
---
- 课程名称：编程语言原理与编译
- 实验项目：期末大作业
- 专业班级：计算机1904
- 学生学号：31901121，31901115
- 学生姓名：余溢轩，王嵊栋
- 实验指导教师: 张芸

## 简介
这是一个编译原理大作业，主要基于microC完成的，这个之所以取名为Cuby，主要是在看`《计算的本质》`这本书的时候，发现Ruby是一门非常好玩有趣的语言，相比C++的错综复杂来说，Ruby是一个集成了优雅与复杂的语言，比如在`irb`下：
```ruby
>> 3.times{puts("Hello world")}
Hello world
Hello world
Hello world
=> 3
>>
```
我看到这门语言的时候我就惊呆了，居然语言还可以这样玩。而C++却令人反胃，实在是太恶心了，虽然说C++给你提供了所有你想用的，但是学习成本高，任何东西都感觉不伦不类的。  
Ruby是一门完全面向对象的编程语言，我尝试去实现面向对象的功能，本来取名叫Yuby，奈何实现面向对象实在太难了，我光是看JVM的指令集就很困难了，还要实现一大堆类库，实在太过于困难了，中途也下过Ruby的源代码，是用C写成的。最后还是放弃了实现面向对象的功能，选择结合microC与Ruby的语法方面作为我们大作业的方向。  
我们打算完善microC并加入Ruby 的 语法，最后如果还有时间的话能够完成面向对象的一个类(最后还是没时间了)。


## 结构
- 前端：由`F#`语言编写而成
  - `CubyLex.fsl`生成的`CubyLex.fs`词法分析器。
  - `CubyPar.fsy`生成的`CubyPar.fs`语法分析器。
  - `AbstractSyntax.fs` 定义了抽象语法树
  - `Assembly.fs`定义了中间表示的生成指令集
  - `Compile.fs`将抽象语法树转化为中间表示

- 后端：由`Java`语言编写而成
  - `Machine.java`生成`Machine.class`虚拟机与`Machinetrace.class`堆栈追踪

- 测试集：测试程序放在`testing`文件夹内

- 库：`.net`支持
  - `FsLexYacc.Runtime.dll`
## 用法

- `fslex --unicode CubyLex.fsl`  
  生成`CubyLex.fs`词法分析器

- `fsyacc --module CubyPar CubyPar.fsy`  
  生成`CubyPar.fs`语法分析器与`CubyPar.fsi`

- `javac Machine.java`  
  生成虚拟机

- `fsi -r FsLexYacc.Runtime.dll AbstractSyntax.fs CubyPar.fs CubyLex.fs Parse.fs Assembly.fs Compile.fs ParseAndComp.fs`  
  可以启用`fsi`的运行该编译器。

- 在`fsi`中输入:  
  `open ParseAndComp;;`

- 之后则可以在`fsi`中使用使用：

  - `fromString`：从字符串中进行编译

  - `fromFile`：从文件中进行编译

  - `compileToFile`：生成中间表示

例子：

```fsharp
compileToFile (fromFile "testing/ex(struct).c") "testing/ex(struct).out";;  
#q;;


fromString "int a;"
```

生成中间表示之后，便可以使用虚拟机对中间代码进行运行得出结果：



虚拟机功能：
- `java Machine` 运行中间表示
- `java Machinetrace` 追踪堆栈变化

例子：
```bash
java Machine ex11.out 8
java Machinetrace ex9.out 0
```

## 功能实现
1:自加加 
  语法树:
    ![](./img/ex(++).jpg)
  运行结果:
    ![](./img/ex(++).1.jpg)
2:自减减
  语法树:
  ![](./img/ex(--).jpg)
  运行结果:
  ![](./img/ex(--).1.jpg)
3:break
  语法树:
  ![](./img/ex(break).jpg)
  运行结果:
  ![](./img/ex(break).1.jpg)
3:continue
  语法树:
  ![](./img/ex(break).jpg)
  运行结果:
  ![](./img/ex(break).1.jpg)
3:switch
  语法树:
  ![](./img/ex(switch).jpg)
  运行结果:
  ![](./img/ex(switch).1.jpg)
## 心得体会
- 余溢轩：  
  本学期的编译原理大作业，对我而言有难度，需要投入大量的时间去学习，了解本质的东西。但这完成大作业的过程，我对编译原理课程的认识更加深刻，掌握的东西也更加多了。
  - 了解了函数式编程的语言特点与特性，拓宽视野。
  - 利用F#与java完善了一个编程语言的从前端到后端的完整的搭建
  - 清楚了一些关于C语言的设计方法与局限性。
  - 理解了栈式虚拟机的工作原理与一些设计方法
  - 利用虚拟机避免与汇编指令集直接打交道，优化代码执行策略

  时间太紧了，如果时间再多一点的话，还可以往虚拟机中加入全局静态变量表、完善结构体、加入头文件机制、加入输入模块等。  
  总而言之，这节编译原理课，收获颇丰，F#好用值得学习。编译原理的世界还是相当有趣的。

- 王嵊栋：  
  这个学期的编译原理大作业，我学习到了很多，理解了基本编译原理，对其他语言也有了一些理解。
  总结一下
  - 大作业完成中，对指令与栈结构有了更深的了解
  - 随着对fsharp的使用越加频繁，也加深了对函数式编程语言的印象
  - 过程的积累中，逐渐了解一些编译的理念，计算的思维。
  - 在大作业有些功能的完成上，还可以再优化精简。时间足够的话还要继续完善更多的异常处理功能，可以增加一些面向对象的思想在里面，对虚拟机的支持要求更高，也可以做些关于垃圾回收的功能。


## 技术评价

| 功能 | 对应文件 | 优  | 良  | 中  |
| ---- | -------- | --- | --- | --- |
|变量声明定义|ex(init).c|√|
|自增、自减|ex(selfplus).c|√|
|for循环|ex(for).c|√|
|do-while|ex(dowhile).c|√|
|while|ex(while).c|√|
|break|ex(break).c|√|
|continue|ex(continue).c|√|
|switch-case|ex(switch).c|√|
|float 类型|ex(float).c|√||
|char 类型|ex(chars).c|√|
|虚拟机类型支持|Machine.java|√|
|虚拟机异常|exception|√

## 小组分工

- 余溢轩
  - 学号：31901121
  - 班级：计算机1904
    - 工作内容
      - 文档编写
      - 测试程序
      - 主要负责虚拟机和中间代码生成
- 王嵊栋
  - 学号：31901115
  - 班级：计算机1904
    - 工作内容
      - 文档编写
      - 测试程序
      - 语法分析
      - 词法分析

- 权重分配表：

| 余溢轩 | 王嵊栋 |
| ---- | ---- |
| 0.95 | 0.95 |

