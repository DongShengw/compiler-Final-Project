rem 根据你的配置修改路径

echo %~n1

dotnet "C:\Users\gm\.nuget\packages\fslexyacc\10.2.0\build\fslex\netcoreapp3.1\fslex.dll"  --module %~n1 --unicode  %1 