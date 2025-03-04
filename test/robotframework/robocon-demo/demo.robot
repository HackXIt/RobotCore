*** Settings ***
Resource    remote-library.resource

*** Variables ***
${VAR_1}    Hello RoboCon
${VAR_2}    2025

*** Test Cases ***
Say Hello
    EXAMPLE.Say Hello    ${VAR_1}    ${VAR_2}