Feature: NotAuthenticated
	As a apprentice who wants to view his commitment statements
	If he is isn't authenticated

Scenario: The apprentice is not authenticated and should be directed to the login page
	Given the apprentice has not logged in
	When first accessing the commitment statement website
	Then the apprentice should be directed to the login page
