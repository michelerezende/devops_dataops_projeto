Feature: Enterprise Calculator System
    As a financial analyst
    I want to use a reliable calculator
    So that I can perform accurate calculations without critical errors

    Background: System initialized
        Given the calculator system is available

    Rule: Addition operations must respect safety limits

        Scenario Outline: Add numbers within allowed limits
            When I add <number1> and <number2>
            Then the result should be <expected_result>
            And the operation should be successful

            Examples:
            | number1 | number2 | expected_result |
            | 10      | 20      | 30              |
            | -5      | 15      | 10              |
            | 0       | 100     | 100             |
            | 999999  | 1       | 1000000         |

        Scenario: Try to add values that exceed individual limit
            When I try to add 1500000 and 500000
            Then the operation should fail
            And I should receive the message "Values cannot exceed 1 million"

        Scenario: Try to add values where result exceeds total limit
            When I try to add 999999 and 999999
            Then the operation should fail
            And I should receive the message "Result exceeds maximum limit of 1.8 million"

    Rule: Division operations must prevent mathematical errors

        Scenario Outline: Divide valid numbers
            When I divide <dividend> by <divisor>
            Then the result should be <expected_result>
            And the operation should be successful

            Examples:
            | dividend | divisor | expected_result |
            | 10       | 2       | 5               |
            | 15       | 3       | 5               |
            | 7        | 2       | 3.5             |
            | -10      | 2       | -5              |

        Scenario: Try to divide by zero
            When I try to divide 10 by 0
            Then the operation should fail
            And I should receive the message "Division by zero is not allowed"

        Scenario: Divide zero by a number
            When I divide 0 by 5
            Then the result should be 0
            And the operation should be successful