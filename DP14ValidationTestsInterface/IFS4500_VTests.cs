using System;
using System.Collections.Generic;
using System.Text;


namespace DP14ValidationTestsInterface
{
    public interface IFS4500_VTests
    {
        List<string> GetTestDescriptions();  

        void Initialize(int testID, List<string> configParameters, int maxReportedErrors = 10);

        List<string> doTest(int testNumber, byte[] stateData);

        List<string> GetTestResultsSummary(int testNumber);

        List<string> GetTestResultsDetailed(int testNumber);

        List<string> GetTestOverview(int testNumber);
    }
}
