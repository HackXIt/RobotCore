*** Settings ***
Suite Setup    Setup suite
Suite Teardown    Teardown suite
#Test Setup    Import remote library
Library    Process

*** Variables ***
${PORT}              8270

*** Keywords ***
Setup suite
    # TODO: Start remote server

Teardown suite
    Run keyword and ignore error    Stop Remote Server
    Terminate Process
    Sleep    3s
    Process Should Be Stopped