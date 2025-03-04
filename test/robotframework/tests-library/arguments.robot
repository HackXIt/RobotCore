*** Settings ***
Resource        remote-library.resource
# Library    Remote    http://localhost:8270/TestKeywords    WITH NAME    TESTLIB
Library         Collections
Library         MockLibrary.py

Suite Setup     Set Log Level    level=DEBUG


*** Variables ***
# Arrays and lists are the same thing in python robot framework.
@{LIST_INT32}                   ${1}    ${2}    ${3}
@{LIST_BOOL}                    ${True}    ${False}    ${True}
@{LIST_DOUBLE}                  ${1.0}    ${0.5}    ${-0.5}
@{LIST_STRING}                  Abc    def    GHI
@{LIST_DICTIONARY_INT32}        &{DICTIONARY_INT32}    &{DICTIONARY_INT32}    &{DICTIONARY_INT32}
@{LIST_DICTIONARY_BOOL}         &{DICTIONARY_BOOL}    &{DICTIONARY_BOOL}    &{DICTIONARY_BOOL}
@{LIST_DICTIONARY_DOUBLE}       &{DICTIONARY_DOUBLE}    &{DICTIONARY_DOUBLE}    &{DICTIONARY_DOUBLE}
@{LIST_DICTIONARY_STRING}       &{DICTIONARY_STRING}    &{DICTIONARY_STRING}    &{DICTIONARY_STRING}

&{DICTIONARY_INT32}             x=${1}    y=${2}    z=${3}
&{DICTIONARY_BOOL}              x=${True}    y=${False}    z=${True}
&{DICTIONARY_DOUBLE}            x=${1.0}    y=${0.5}    z=${-0.5}
&{DICTIONARY_STRING}            x=Abc    y=def    z=GHI
&{DICTIONARY_LIST_INT32}        x=@{LIST_INT32}    y=@{LIST_INT32}    z=@{LIST_INT32}
&{DICTIONARY_LIST_BOOL}         x=@{LIST_BOOL}    y=@{LIST_BOOL}    z=@{LIST_BOOL}
&{DICTIONARY_LIST_DOUBLE}       x=@{LIST_DOUBLE}    y=@{LIST_DOUBLE}    z=@{LIST_DOUBLE}
&{DICTIONARY_LIST_STRING}       x=@{LIST_STRING}    y=@{LIST_STRING}    z=@{LIST_STRING}

${isList}                       ${False}
${Abc}                          Abc


*** Test Cases ***
Explicit Multiple Int32 Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${1}
    ...    arg2=${2}
    ...    arg3=${3}
    ...    arg4=${4}
    ...    arg5=${5}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS INT32
    ...    ${1}
    ...    ${2}
    ...    ${3}
    ...    ${4}
    ...    ${5}
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple Boolean Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${True}
    ...    arg2=${False}
    ...    arg3=${True}
    ...    arg4=${False}
    ...    arg5=${True}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS BOOLEAN
    ...    ${True}
    ...    ${False}
    ...    ${True}
    ...    ${False}
    ...    ${True}
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple Double Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${1.0}
    ...    arg2=${0.5}
    ...    arg3=${-0.5}
    ...    arg4=${1.0}
    ...    arg5=${0.5}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS DOUBLE
    ...    ${1.0}
    ...    ${0.5}
    ...    ${-0.5}
    ...    ${1.0}
    ...    ${0.5}
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple String Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=abc
    ...    arg2=def
    ...    arg3=GHI
    ...    arg4=abc
    ...    arg5=def
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS STRING
    ...    abc
    ...    def
    ...    GHI
    ...    abc
    ...    def
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple Mixed Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=abc
    ...    arg2=${1}
    ...    arg3=${True}
    ...    arg4=${-1}
    ...    arg5=${1.0}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS MIXED
    ...    abc
    ...    ${1}
    ...    ${True}
    ...    ${-1}
    ...    ${1.0}
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple Int32 Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${1}
    ...    arg2=${2}
    ...    arg3=${3}
    ...    arg4=${4}
    ...    arg5=${5}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS INT32
    ...    1
    ...    2
    ...    3
    ...    4
    ...    5
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple Boolean Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${True}
    ...    arg2=${False}
    ...    arg3=${True}
    ...    arg4=${False}
    ...    arg5=${True}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS BOOLEAN
    ...    True
    ...    False
    ...    True
    ...    False
    ...    True
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple Double Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${1.0}
    ...    arg2=${0.5}
    ...    arg3=${-0.5}
    ...    arg4=${1.0}
    ...    arg5=${0.5}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS DOUBLE
    ...    1.0
    ...    0.5
    ...    -0.5
    ...    1.0
    ...    0.5
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple String Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=abc
    ...    arg2=def
    ...    arg3=GHI
    ...    arg4=abc
    ...    arg5=def
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS STRING
    ...    abc
    ...    def
    ...    GHI
    ...    abc
    ...    def
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple Mixed Types Positional
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=abc
    ...    arg2=1
    ...    arg3=True
    ...    arg4=-1
    ...    arg5=1.0
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS MIXED
    ...    abc
    ...    1
    ...    True
    ...    -1
    ...    1.0
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple Int32 Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${1}
    ...    arg2=${2}
    ...    arg3=${3}
    ...    arg4=${4}
    ...    arg5=${5}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS INT32
    ...    arg1=${1}
    ...    arg2=${2}
    ...    arg3=${3}
    ...    arg4=${4}
    ...    arg5=${5}
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple Boolean Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${True}
    ...    arg2=${False}
    ...    arg3=${True}
    ...    arg4=${False}
    ...    arg5=${True}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS BOOLEAN
    ...    arg1=${True}
    ...    arg2=${False}
    ...    arg3=${True}
    ...    arg4=${False}
    ...    arg5=${True}
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple Double Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${1.0}
    ...    arg2=${0.5}
    ...    arg3=${-0.5}
    ...    arg4=${1.0}
    ...    arg5=${0.5}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS DOUBLE
    ...    arg1=${1.0}
    ...    arg2=${0.5}
    ...    arg3=${-0.5}
    ...    arg4=${1.0}
    ...    arg5=${0.5}
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple String Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=abc
    ...    arg2=def
    ...    arg3=GHI
    ...    arg4=abc
    ...    arg5=def
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS STRING
    ...    arg1=abc
    ...    arg2=def
    ...    arg3=GHI
    ...    arg4=abc
    ...    arg5=def
    Should Be Equal    ${actualString}    ${expectedString}

Explicit Multiple Mixed Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=abc
    ...    arg2=${1}
    ...    arg3=${True}
    ...    arg4=${-1}
    ...    arg5=${1.0}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS MIXED
    ...    arg1=abc
    ...    arg2=${1}
    ...    arg3=${True}
    ...    arg4=${-1}
    ...    arg5=${1.0}
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple Int32 Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${1}
    ...    arg2=${2}
    ...    arg3=${3}
    ...    arg4=${4}
    ...    arg5=${5}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS INT32
    ...    arg1=1
    ...    arg2=2
    ...    arg3=3
    ...    arg4=4
    ...    arg5=5
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple Boolean Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${True}
    ...    arg2=${False}
    ...    arg3=${True}
    ...    arg4=${False}
    ...    arg5=${True}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS BOOLEAN
    ...    arg1=True
    ...    arg2=False
    ...    arg3=True
    ...    arg4=False
    ...    arg5=True
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple Double Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=${1.0}
    ...    arg2=${0.5}
    ...    arg3=${-0.5}
    ...    arg4=${1.0}
    ...    arg5=${0.5}
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS DOUBLE
    ...    arg1=1.0
    ...    arg2=0.5
    ...    arg3=-0.5
    ...    arg4=1.0
    ...    arg5=0.5
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple String Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=abc
    ...    arg2=def
    ...    arg3=GHI
    ...    arg4=abc
    ...    arg5=def
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS STRING
    ...    arg1=abc
    ...    arg2=def
    ...    arg3=GHI
    ...    arg4=abc
    ...    arg5=def
    Should Be Equal    ${actualString}    ${expectedString}

Implicit Multiple Mixed Types Named Arguments
    ${expectedString}=    Catenate    SEPARATOR=\n
    ...    arg1=abc
    ...    arg2=1
    ...    arg3=True
    ...    arg4=-1
    ...    arg5=1.0
    ${actualString}=    TESTLIB.MULTIPLEPARAMETERS MIXED
    ...    arg1=abc
    ...    arg2=1
    ...    arg3=True
    ...    arg4=-1
    ...    arg5=1.0
    Should Be Equal    ${actualString}    ${expectedString}
