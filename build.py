#! /usr/bin/env python3

import os
import os.path
import shutil
import subprocess


def abspath(path):
    home = os.path.dirname(__file__)
    return os.path.normpath(os.path.join(home, path))


def dotnet(*args):
    process = ["dotnet"] + list(args)
    subprocess.run(process)


shutil.rmtree(abspath('build'), ignore_errors=True)
dotnet('build', abspath('src/FactoryFactory.sln'))
dotnet('test', abspath('src/FactoryFactory.Tests/FactoryFactory.Tests.csproj'))
dotnet(
    'pack',
    '-o', abspath('build/FactoryFactory'),
    '--no-build',
    abspath('src/FactoryFactory/FactoryFactory.csproj')
)
