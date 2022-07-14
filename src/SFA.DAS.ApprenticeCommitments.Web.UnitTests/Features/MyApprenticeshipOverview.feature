@outerApi
Feature: MyApprenticeshipOverview

Feature: MyApprenticeships
	As an apprentice
	I need to have a start screen showing all the steps of my commitment statement
	So that I know what steps are involved in signing my commitment statement
	And I can be directed to sign every aspect of my statement

Scenario: The apprentice is authenticated and should see the overview page
	Given the apprentice has logged in
	And the apprentice will navigate to the overview page
	When accessing the overview page
	Then the response status code should be Ok
	And the apprentice should not see the change notification banner
	And the apprenticeship revision is updated with the time the page was viewed

Scenario: The apprentice has not confirmed every aspect of the apprenciceship
	Given the apprentice has logged in
	And the apprentice has not confirmed every aspect of the apprenciceship	
	And the apprentice will navigate to the overview page
	When accessing the overview page
	Then the response status code should be Ok
	Then the apprentice should not see the ready to confirm banner

Scenario: The apprentice has confirmed every aspect of the apprenticeship
	Given the apprentice has logged in
	And the apprentice has confirmed every aspect of the apprenticeship
	And the apprentice will navigate to the overview page
	When accessing the overview page
	Then the response status code should be Ok
	Then the apprentice should see the ready to confirm banner

Scenario: The apprentice is shown days remaining for confirmation
	Given the apprentice has logged in
	And the confirmation deadline is <Confirm Before>
	And the time is <Now>
	And the apprentice will navigate to the overview page
	When accessing the overview page
	Then the response status code should be Ok
	And the apprentice should see <Days Remaining> remaining
	And the overdue state should be <Overdue>

	Examples: 
	| Confirm Before      | Now                 | Days Remaining | Overdue |
	| 2021-07-12T18:20:00 | 2021-06-29T03:45:00 | 14 days        | false   |
	| 2021-03-25 10:59    | 2021-03-12 10:59    | 14 days        | false   |
	| 2021-03-25 10:59    | 2021-03-12 11:00    | 13 days        | false   |
	| 2021-03-25 10:59    | 2021-03-25 10:59    | 1 day          | false   |
	| 2021-03-25 10:59    | 2021-03-26 10:59    | 0 days         | true    |
	| 2021-03-25 10:59    | 2021-03-27 10:59    | 0 days         | true    |

Scenario: The apprentice is shown the change of circumstances notification
	Given the apprentice has logged in
	And the apprenticeship has changes to these sections : <Provider Changed>, <Employer Changed>, <Apprenticeship Changed> 
	And the apprentice will navigate to the overview page
	When accessing the overview page
	Then the response status code should be Ok
	And the apprentice should see the change notification banner
	And the message starts like <Expected Message Starts Like>

		Examples: 
	| Provider Changed | Employer Changed | Apprenticeship Changed | Delivery Model Changed | Expected Message Starts Like                                |
	| true             | false            | false                  | false					| The details of your apprenticeship have been updated        |
	| true             | true             | false                  | false					| The details of your apprenticeship have been updated        |
	| true             | true             | true                   | false					| The details of your apprenticeship have been updated        |
	| false            | true             | false                  | false					| The details of your apprenticeship have been updated        |
	| false            | false            | true                   | false					| The details of your apprenticeship have been updated        |
	| false            | true             | true                   | false					| The details of your apprenticeship have been updated        |
	| false            | true             | true                   | true					| The details of your apprenticeship have been updated        |

Scenario: The apprentice's apprenticeship is stopped
	Given the apprentice has logged in
	And the apprenticeship is stopped
	And the apprentice will navigate to the overview page
	When accessing the overview page
	Then the response status code should be Ok
	And the apprenticeship should be stopped
