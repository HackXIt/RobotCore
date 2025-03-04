'''
File: ComplexTypeConverter.py
Created on: Friday, 2023-09-15 @ 10:44:52
Author: Nikolaus Rieder (<n.rieder@schrack-seconet.com>)
-----
Last Modified: Tuesday, 2023-09-19 @ 12:55:25
Modified By:  Nikolaus Rieder (<n.rieder@schrack-seconet.com>) @ SE6802S
-----
'''

from robot import libraries
from robot.api.deco import keyword, library

@library
class ComplexTypeConverter:
    def __init__(self, remote_lib):
        # Assuming the remote library is already initialized and connected
        self.remote_lib = libraries.Remote.Remote(remote_lib)
        self.remote_lib_name = remote_lib

    @keyword('Convert and Validate')
    def convert_and_validate(self, data):
        # Determine data type and create function name
        if isinstance(data, list):
            # Let's determine the list data type
            item_type = self._determine_list_type(data)
            func_name = f'List{item_type} ParameterType'
        elif isinstance(data, dict):
            # Let's determine the dictionary value type
            value_type = self._determine_dict_value_type(data)
            func_name = f'Dictionary{value_type} ParameterType'
        else:
            raise TypeError("Unsupported data type")

        # Call the respective remote function
        result = self.remote_lib.run_keyword(func_name, [data], None)
        return result

    @keyword('Is List')
    def is_list(self, data):
        return isinstance(data, list)

    @keyword('Is Dictionary')
    def is_dict(self, data):
        return isinstance(data, dict)

    def _determine_list_type(self, data_list):
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

    def _determine_dict_value_type(self, data_dict):
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
