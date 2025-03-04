'''
File: xml-rpc-client.py
Created on: Friday, 2023-09-15 @ 14:45:14
Author: Nikolaus Rieder (<n.rieder@schrack-seconet.com>)
-----
Last Modified: Monday, 2023-10-23 @ 08:10:05
Modified By:  Nikolaus Rieder (<n.rieder@schrack-seconet.com>) @ SE6802S
-----
'''
import xmlrpc.client
import pprint

intList = [1, 2, 3, 4, 5]
boolList = [True, False, True, False, True]
floatList = [1.1, 2.2, 3.3, 4.4, 5.5]
strList = ['a', 'b', 'c', 'd', 'e']

intDict = {'a': 1, 'b': 2, 'c': 3, 'd': 4, 'e': 5}
boolDict = {'a': True, 'b': False, 'c': True, 'd': False, 'e': True}
floatDict = {'a': 1.1, 'b': 2.2, 'c': 3.3, 'd': 4.4, 'e': 5.5}
strDict = {'a': 'a', 'b': 'b', 'c': 'c', 'd': 'd', 'e': 'e'}
mixed = ["mixedArg", 2, 3.3, True, "optional", 99]

data = [intList, boolList, floatList, strList, intDict, boolDict, floatDict, strDict]


def execute_keyword(uri, keyword, args, kwargs):
    with xmlrpc.client.ServerProxy(uri, encoding='UTF-8', 
                                   use_builtin_types=True, 
                                   verbose=True) as proxy:
        pprint.pprint(proxy)
        try:
            run_keyword_args = [keyword, args, kwargs] if kwargs else [keyword, args]
            pprint.pprint(proxy.run_keyword(*run_keyword_args))
        except xmlrpc.client.Fault as err:
            print(err)


def get_library(uri):
    with xmlrpc.client.ServerProxy(uri, encoding='UTF-8',
                                   use_builtin_types=True,
                                   verbose=True) as proxy:
        pprint.pprint(proxy)
        try:
            pprint.pprint(proxy.get_library_information())
        except xmlrpc.client.Fault as err:
            print(err)


def _determine_list_type(data_list):
    # Assuming lists are homogeneous
    if all(isinstance(item, int) for item in data_list):
        return 'Int32'
    elif all(isinstance(item, bool) for item in data_list):
        return 'Boolean'
    elif all(isinstance(item, float) for item in data_list):
        return 'Double'
    elif all(isinstance(item, str) for item in data_list):
        return 'String'
    else:
        raise TypeError("Unsupported list item type")


def _determine_dict_value_type(data_dict):
    # Assuming dictionary values are homogeneous
    sample_value = next(iter(data_dict.values()))
    if isinstance(sample_value, int):
        return 'Int32'
    elif isinstance(sample_value, bool):
        return 'Boolean'
    elif isinstance(sample_value, float):
        return 'Double'
    elif isinstance(sample_value, str):
        return 'String'
    else:
        raise TypeError("Unsupported dictionary value type")


if __name__ == "__main__":
    # for item in data:
    #     if isinstance(item, list):
    #         # Let's determine the list data type
    #         item_type = _determine_list_type(item)
    #         func_name = f'List{item_type} ParameterType'
    #     elif isinstance(item, dict):
    #         # Let's determine the dictionary value type
    #         value_type = _determine_dict_value_type(item)
    #         func_name = f'Dictionary{value_type} ParameterType'
    #     else:
    #         raise TypeError("Unsupported data type")
    #     execute_keyword('http://localhost:8270/Testcenter/RobotFramework/Test/KeywordLibrary/TestKeywords', func_name, item)    
    # get_library('http://localhost:8270/Testcenter/RobotFramework/Test/KeywordLibrary/TestKeywords')
    # get_library('http://localhost:8270/Testcenter/RobotLibrary/HCS/Visocall_IP/VisocallIpKeywords')
    get_library('http://localhost:8270/ExampleLibrary')
    # func_name = 'ListDictionaryInt32 ParameterType'
    # test_library= 'http://localhost:8270/Testcenter/RobotFramework/Test/KeywordLibrary/TestKeywords'
    # execute_keyword(test_library, func_name, [5], None)
    # get_library(test_library)
