from imghdr import tests
import requests
import json
import os.path as path
import sys
from urllib3.exceptions import InsecureRequestWarning
import concurrent.futures

requests.packages.urllib3.disable_warnings(category=InsecureRequestWarning)
tests_scenarios = [("complete_test", 201), ("partial_test", 201), ("invalid_test", 400)]
tests = []
endpoint = ""
class Test:
    def __init__(self, request, expected_result, name):
        self.name = name
        self.request = request
        self.expected_result = expected_result

def import_tests_from_path(tests_path):
    for (test_name, expected_result) in tests_scenarios:
        with open(path.join(tests_path, f"{test_name}.json"), "r") as f:
            request_body = json.load(f)
            tests.append(Test(request_body, expected_result, test_name))

def run_test(test):
    response = requests.post(endpoint, json=test.request, verify=False, timeout=100)
    print (response.elapsed.total_seconds())
    if response.status_code != test.expected_result:
        print(f"[FAILED] Test {test.name} failed with status code {response.status_code}")
        sys.exit(1)
    print(f"[PASSED] Test {test.name}.")

if __name__ == "__main__":
    print("Starting e2et tests")
    if len(sys.argv) != 3:
        print("Usage: e2et_test.py <tests_path> <endpoint>")
        sys.exit(1)
    tests_path = sys.argv[1]
    endpoint = sys.argv[2]
    import_tests_from_path(tests_path)
    for test in tests:
        run_test(test)
    print("Everything passed")
