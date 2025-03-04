*** Settings ***
Default Tags  argsknown
Resource    resource.robot

*** Test Cases ***

Using Arguments When No Accepted
    [Documentation]  FAIL Keyword 'Remote.No Arguments' expected 0 arguments, got 1.
    No Arguments  not allowed

Too Few Arguments When Using Only Required Args
    [Documentation]  FAIL Keyword 'Remote.One Argument' expected 1 argument, got 0.
    One Argument

Too Many Arguments When Using Only Required Args
    [Documentation]  FAIL Keyword 'Remote.Two Arguments' expected 2 arguments, got 3.
    Two Arguments    too    many    arguments

Too Few Arguments When Using Default Values
    [Documentation]  FAIL Keyword 'Remote.Arguments With Default Values' expected 1 to 3 arguments, got 0.
    Arguments With Default Values

Too Many Arguments When Using Default Values
    [Documentation]  FAIL Keyword 'Remote.Arguments With Default Values' expected 1 to 3 arguments, got 5.
    Arguments With Default Values    this    is    way    too    much

Too Few Arguments When Using Varargs
    [Documentation]  FAIL Keyword 'Remote.Required Defaults And Varargs' expected at least 1 argument, got 0.
    Required Defaults And Varargs