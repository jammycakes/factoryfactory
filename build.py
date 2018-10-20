#! /usr/bin/env python3

import os
import os.path
import re
import shutil
import subprocess

version = '0.0.1-alpha'


def abspath(path):
    home = os.path.dirname(__file__)
    return os.path.normpath(os.path.join(home, path))


def dotnet(*args):
    process = ["dotnet"] + list(args)
    subprocess.run(process)


build_number = os.environ.get('APPVEYOR_BUILD_NUMBER', '0')
pull_request_number = os.environ.get('APPVEYOR_PULL_REQUEST_NUMBER', False)
suffix = ''
is_release = False

if os.environ.get('APPVEYOR_REPO_TAG', False) == 'true':
    version = os.environ.get('APPVEYOR_REPO_TAG_NAME')
    is_release = True

version_number = re.search(r'^\d+\.\d+\.\d+', version)
if version_number:
    version_number = version_number.group()
    file_version = version_number + '.' + build_number
else:
    file_version = False

if pull_request_number:
    package_version = version + '-pr' + pull_request_number
else:
    package_version = version

os.makedirs(abspath('src/.version'), exist_ok=True)
with open(abspath('src/.version/version.cs'), 'w') as f:
    f.writelines([
        'using System.Reflection;\n'
        '\n',
        '[assembly:AssemblyInformationalVersion("{0}")]\n'.format(package_version)
    ])
    if file_version:
        f.writelines([
            '[assembly:AssemblyVersion("{0}")]\n'.format(file_version),
            '[assembly:AssemblyFileVersion("{0}")]\n'.format(file_version)
        ])

shutil.rmtree(abspath('build'), ignore_errors=True)
dotnet('build', abspath('src/FactoryFactory.sln'))
dotnet('test', abspath('src/FactoryFactory.Tests/FactoryFactory.Tests.csproj'))
dotnet(
    'pack',
    '-o', abspath('build/FactoryFactory'),
    '--no-build',
    abspath('src/FactoryFactory/FactoryFactory.csproj')
)
