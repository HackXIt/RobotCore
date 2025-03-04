*** Settings ***
Resource    remote-library.resource
Library     ComplexTypeConverter.py    http://localhost:8270/TestKeywords    WITH NAME    TypeConverter
Library     Collections
Library     MockLibrary.py


*** Variables ***
# Arrays and lists are the same thing in python robot framework.
@{LIST_INT32}           ${1}    ${2}    ${3}
@{LIST_BOOL}            ${True}    ${False}    ${True}
@{LIST_DOUBLE}          ${1.0}    ${0.5}    ${-0.5}
@{LIST_STRING}          Abc    def    GHI

&{DICTIONARY_INT32}     x=${1}    y=${2}    z=${3}
&{DICTIONARY_BOOL}      x=${True}    y=${False}    z=${True}
&{DICTIONARY_DOUBLE}    x=${1.0}    y=${0.5}    z=${-0.5}
&{DICTIONARY_STRING}    x=Abc    y=def    z=GHI

${isList}               ${False}


*** Test Cases ***
ComplexTypeConversion
    [Documentation]    This test case demonstrates the conversion of complex types from robot framework to the RobotService and also back to robot framework.
    @{type_names}=    Create List    Int32    Bool    Double    String
    ${data}=    Create List
    ...    ${LIST_INT32}
    ...    ${LIST_BOOL}
    ...    ${LIST_DOUBLE}
    ...    ${LIST_STRING}
    ...    ${DICTIONARY_INT32}
    ...    ${DICTIONARY_BOOL}
    ...    ${DICTIONARY_DOUBLE}
    ...    ${DICTIONARY_STRING}

    FOR    ${element}    IN    @{data}
        Log    ${element}
        ConvertAndValidateCustom    ${element}
    END


*** Keywords ***
ConvertAndValidateCustom
    [Arguments]    ${dataObj}
    # ${isList}=    TypeConverter.Is List    ${dataObj}
    ${isList}=    Run Keyword And Warn On Failure    TypeConverter.Is List    ${dataObj}
    ${isDictionary}=    Run Keyword And Warn On Failure    TypeConverter.Is Dictionary    ${dataObj}
    IF    ${isList[1]}
        Log    message=${dataObj}    console=${True}
        @{result}=    Run Keyword And Warn On Failure    TypeConverter.Convert and Validate    ${dataObj}
        Log    message=${result[1]}    console=${True}
        Run Keyword And Warn On Failure    Lists Should Be Equal    ${result[1]}    ${dataObj}
    ELSE IF    ${isDictionary[1]}
        Log    message=${dataObj}    console=${True}
        &{result}=    Run Keyword And Warn On Failure    TypeConverter.Convert and Validate    ${dataObj}
        Log    message=${result[1]}    console=${True}
        Run Keyword And Warn On Failure    Dictionaries Should Be Equal    ${result[1]}    ${dataObj}
    END
