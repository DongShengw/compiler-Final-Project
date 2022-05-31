// 解释器
dotnet "C:\Users\栋晟\.nuget\packages\fslexyacc\10.2.0\build\fslex\netcoreapp3.1\fslex.dll" -o "CLex.fs" --module CLex --unicode CLex.fsl

#r "nuget: FsLexYacc";;  //添加包引用
#load "Absyn.fs" "Debug.fs" "CPar.fs" "CLex.fs" "Parse.fs" "Interp.fs" "ParseAndRun.fs" ;;

open ParseAndRun;;    //导入模块 ParseAndRun
fromFile "ex1.c";;    //显示 ex1.c的语法树
run (fromFile "ex1.c") [17];; //解释执行 ex1.c
run (fromFile "ex11.c") [8];; //解释执行 ex11.c

Debug.debug <-  true  //打开调试

run (fromFile "ex1.c") [8];; //解释执行 ex1.c
run (fromFile "ex11.c") [8];; //解释执行 ex11.c

//编译器
dotnet "C:\Users\栋晟\.nuget\packages\fslexyacc\10.2.0\build\fsyacc\netcoreapp3.1\fsyacc.dll" -o "CPar.fs" --module CPar  CPar.fsy
#r "nuget: FsLexYacc";;

#load "Absyn.fs"  "CPar.fs" "CLex.fs" "Debug.fs" "Parse.fs" "StackMachine.fs" "Backend.fs" "Comp.fs" "ParseAndComp.fs";;

//运行编译器
open ParseAndComp;;
compileToFile (fromFile "ex11.c") "ex11";;  //生成机器码ex11.out

Debug.debug <-  true  //打开调试
compileToFile (fromFile "ex4.c") "ex4";;  // 注意变量分配

//优化编译器
#r "nuget: FsLexYacc";;

#load "Absyn.fs"  "CPar.fs" "CLex.fs" "Debug.fs" "Parse.fs" "StackMachine.fs" "Backend.fs" "ContComp.fs" "ParseAndContcomp.fs";;

open ParseAndContcomp;;
contCompileToFile (fromFile "ex11.c") "ex11.out";;

