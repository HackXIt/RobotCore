*** Settings ***
Resource    remote-library.resource
# Library    Remote    http://localhost:8270/TestKeywords    WITH NAME    TESTLIB
Library     Collections
Library     MockLibrary.py
Suite Setup     Set Log Level    level=DEBUG


*** Variables ***
# Arrays and lists are the same thing in python robot framework.
@{LIST_INT32}           ${1}    ${2}    ${3}
@{LIST_BOOL}            ${True}    ${False}    ${True}
@{LIST_DOUBLE}          ${1.0}    ${0.5}    ${-0.5}
@{LIST_STRING}          Abc    def    GHI
@{LIST_DICTIONARY_INT32}    &{DICTIONARY_INT32}    &{DICTIONARY_INT32}    &{DICTIONARY_INT32}
@{LIST_DICTIONARY_BOOL}     &{DICTIONARY_BOOL}    &{DICTIONARY_BOOL}    &{DICTIONARY_BOOL}    
@{LIST_DICTIONARY_DOUBLE}   &{DICTIONARY_DOUBLE}    &{DICTIONARY_DOUBLE}    &{DICTIONARY_DOUBLE}
@{LIST_DICTIONARY_STRING}   &{DICTIONARY_STRING}    &{DICTIONARY_STRING}    &{DICTIONARY_STRING}

&{DICTIONARY_INT32}     x=${1}    y=${2}    z=${3}
&{DICTIONARY_BOOL}      x=${True}    y=${False}    z=${True}
&{DICTIONARY_DOUBLE}    x=${1.0}    y=${0.5}    z=${-0.5}
&{DICTIONARY_STRING}    x=Abc    y=def    z=GHI
&{DICTIONARY_LIST_INT32}    x=@{LIST_INT32}    y=@{LIST_INT32}    z=@{LIST_INT32}
&{DICTIONARY_LIST_BOOL}     x=@{LIST_BOOL}    y=@{LIST_BOOL}    z=@{LIST_BOOL}
&{DICTIONARY_LIST_DOUBLE}   x=@{LIST_DOUBLE}    y=@{LIST_DOUBLE}    z=@{LIST_DOUBLE}
&{DICTIONARY_LIST_STRING}   x=@{LIST_STRING}    y=@{LIST_STRING}    z=@{LIST_STRING}

${isList}               ${False}
${Abc}                  Abc


*** Test Cases ***
Explicit Int32 Type
    ${converted_int}=    TESTLIB.Int32 ParameterType    ${1}
    Should Be Equal    ${converted_int}    ${1}

Explicit Boolean Type
    ${converted_bool}=    TESTLIB.Boolean ParameterType    ${True}
    Should Be Equal    ${converted_bool}    ${True}

Explicit Double Type
    ${converted_double}=    TESTLIB.Double ParameterType    ${1.0}
    Should Be Equal    ${converted_double}    ${1.0}

Explicit String Type
    ${converted_string}=    TESTLIB.String ParameterType    ${Abc}
    Should Be Equal    ${converted_string}    ${Abc}

Implicit Int32 Type
    ${converted_int}=    TESTLIB.Int32 ParameterType    1
    Should Be Equal    ${converted_int}    ${1}

Implicit Boolean Type
    ${converted_bool}=    TESTLIB.Boolean ParameterType    True
    Should Be Equal    ${converted_bool}    ${True}

Implicit Double Type
    ${converted_double}=    TESTLIB.Double ParameterType    1.0
    Should Be Equal    ${converted_double}    ${1.0}

Implicit String Type
    ${converted_string}=    TESTLIB.String ParameterType    Abc
    Should Be Equal    ${converted_string}    Abc

Explicit Multiple Int32 Types
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

Explicit Multiple Boolean Types
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

Explicit Multiple Double Types
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

Explicit Multiple String Types
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

Explicit Multiple Mixed Types
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

Implicit Multiple Int32 Types
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

Implicit Multiple Boolean Types
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

Implicit Multiple Double Types
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

Implicit Multiple String Types
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

Implicit Multiple Mixed Types
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

Array Int32 Conversion
    ${converted_list}=    TESTLIB.ArrayInt32 ParameterType    ${LIST_INT32}
    Lists Should Be Equal    ${converted_list}    ${LIST_INT32}

Array Boolean Conversion
    ${converted_list}=    TESTLIB.ArrayBoolean ParameterType    ${LIST_BOOL}
    Lists Should Be Equal    ${converted_list}    ${LIST_BOOL}

Array Double Conversion
    ${converted_list}=    TESTLIB.ArrayDouble ParameterType    ${LIST_DOUBLE}
    Lists Should Be Equal    ${converted_list}    ${LIST_DOUBLE}

Array String Conversion
    ${converted_list}=    TESTLIB.ArrayString ParameterType    ${LIST_STRING}
    Lists Should Be Equal    ${converted_list}    ${LIST_STRING}

List Int32 Conversion
    ${converted_list}=    TESTLIB.ListInt32 ParameterType    ${LIST_INT32}
    Lists Should Be Equal    ${converted_list}    ${LIST_INT32}

List Boolean Conversion
    ${converted_list}=    TESTLIB.ListBoolean ParameterType    ${LIST_BOOL}
    Lists Should Be Equal    ${converted_list}    ${LIST_BOOL}

List Double Conversion
    ${converted_list}=    TESTLIB.ListDouble ParameterType    ${LIST_DOUBLE}
    Lists Should Be Equal    ${converted_list}    ${LIST_DOUBLE}

List String Conversion
    ${converted_list}=    TESTLIB.ListString ParameterType    ${LIST_STRING}
    Lists Should Be Equal    ${converted_list}    ${LIST_STRING}

List Dictionary Int32 Conversion
    ${converted_list}=    TESTLIB.ListDictionaryInt32 ParameterType    ${LIST_DICTIONARY_INT32}
    Lists Should Be Equal    ${converted_list}    ${LIST_DICTIONARY_INT32}

List Dictionary Boolean Conversion
    ${converted_list}=    TESTLIB.ListDictionaryBoolean ParameterType    ${LIST_DICTIONARY_BOOL}
    Lists Should Be Equal    ${converted_list}    ${LIST_DICTIONARY_BOOL}

List Dictionary Double Conversion
    ${converted_list}=    TESTLIB.ListDictionaryDouble ParameterType    ${LIST_DICTIONARY_DOUBLE}
    Lists Should Be Equal    ${converted_list}    ${LIST_DICTIONARY_DOUBLE}

List Dictionary String Conversion
    ${converted_list}=    TESTLIB.ListDictionaryString ParameterType    ${LIST_DICTIONARY_STRING}
    Lists Should Be Equal    ${converted_list}    ${LIST_DICTIONARY_STRING}

Dictionary Int32 Conversion
    ${converted_dict}=    TESTLIB.DictionaryInt32 ParameterType    ${DICTIONARY_INT32}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_INT32}

Dictionary Int32 Multiple Conversion
    ${converted_dict}=    TESTLIB.DictionaryInt32 ParameterType Multiple    ${DICTIONARY_INT32}    ${5}
    ${DICTIONARY_INT32.other}=    Set Variable    ${5}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_INT32}

Dictionary Boolean Conversion
    ${converted_dict}=    TESTLIB.DictionaryBoolean ParameterType    ${DICTIONARY_BOOL}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_BOOL}

Dictionary Double Conversion
    ${converted_dict}=    TESTLIB.DictionaryDouble ParameterType    ${DICTIONARY_DOUBLE}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_DOUBLE}

Dictionary String Conversion
    ${converted_dict}=    TESTLIB.DictionaryString ParameterType    ${DICTIONARY_STRING}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_STRING}

Dictionary List Int32 Conversion
    ${converted_dict}=    TESTLIB.DictionaryListInt32 ParameterType    ${DICTIONARY_LIST_INT32}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_LIST_INT32}

Dictionary List Boolean Conversion
    ${converted_dict}=    TESTLIB.DictionaryListBoolean ParameterType    ${DICTIONARY_LIST_BOOL}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_LIST_BOOL}

Dictionary List Double Conversion
    ${converted_dict}=    TESTLIB.DictionaryListDouble ParameterType    ${DICTIONARY_LIST_DOUBLE}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_LIST_DOUBLE}

Dictionary List String Conversion
    ${converted_dict}=    TESTLIB.DictionaryListString ParameterType    ${DICTIONARY_LIST_STRING}
    Dictionaries Should Be Equal    ${converted_dict}    ${DICTIONARY_LIST_STRING}

# Using the mock library to check types and values

List Mock Example
    Check Var Type    ${LIST_BOOL}

List Mock Example 2
    Check Var List    @{LIST_BOOL}

Dictionary Mock Example
    Check Var Type    ${DICTIONARY_STRING}

# Checking library information

Verify library information
    ${testLib}=    Get Library Instance    TESTLIB
    Reload Library    TESTLIB
    Log    ${testLib}
