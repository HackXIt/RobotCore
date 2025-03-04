class MockLibrary:
    def check_var_type(self, var):
        print("Type:", type(var), "	Value:", var)

    def check_var_list(self, *args):
        print("Type:", type(args), "	Value:", args)
        for var in args:
            print("Type:", type(var), "	Value:", var)
