using System;
using System.Collections.Generic;
using NSubstitute;
using Server.BusinessInterfaces.FieldDataPlugInCore.Context;
using Server.Plugins.FieldVisit.PocketGauger.Helpers;

namespace Server.Plugins.FieldVisit.PocketGauger.UnitTests.TestData
{
    public class ParseContextTestHelper
    {
        public ILocationInfo LocationInfo { get; private set; }
        public IParameter DischargeParameter { get; private set; }
        public IParameter GageHeightParameter { get; private set; }

        private readonly IParseContext _parseContext;

        public ParseContextTestHelper()
        {
            _parseContext = Substitute.For<IParseContext>();
        }

        public IParseContext CreateMockParseContext()
        {
            return CreateMockParseContext(new List<IFieldVisitInfo>());
        }

        public IParseContext CreateMockParseContext(IEnumerable<IFieldVisitInfo> fieldVisitsToReturn)
        {
            SetUpLocation(fieldVisitsToReturn);
            SetUpDischargeParameter();
            SetUpGageHeightParameter();
            SetUpOtherParameters();

            return _parseContext;
        }

        private void SetUpLocation(IEnumerable<IFieldVisitInfo> fieldVisitsToReturn)
        {
            LocationInfo = Substitute.For<ILocationInfo>();
            LocationInfo.FindLocationFieldVisitsInTimeRange(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                .Returns(fieldVisitsToReturn);

            _parseContext.FindLocationByIdentifier(Arg.Any<string>()).Returns(LocationInfo);
        }

        private void SetUpDischargeParameter()
        {
            DischargeParameter = Substitute.For<IParameter>();

            var defaultDischargeUnit = Substitute.For<IUnit>();
            DischargeParameter.DefaultUnit.Returns(defaultDischargeUnit);

            var midSectionMethod = Substitute.For<IMonitoringMethod>();
            midSectionMethod.MethodCode.Returns(ParametersAndMethodsConstants.MidSectionMonitoringMethod);
            var meanSectionMethod = Substitute.For<IMonitoringMethod>();
            meanSectionMethod.MethodCode.Returns(ParametersAndMethodsConstants.MeanSectionMonitoringMethod);
            var defaultDischargeMethod = Substitute.For<IMonitoringMethod>();
            defaultDischargeMethod.MethodCode.Returns(ParametersAndMethodsConstants.DefaultMonitoringMethod);

            DischargeParameter.MonitoringMethods.Returns(new[]
                {midSectionMethod, meanSectionMethod, defaultDischargeMethod});

            _parseContext.DischargeParameter.Returns(DischargeParameter);
        }

        private void SetUpGageHeightParameter()
        {
            GageHeightParameter = Substitute.For<IParameter>();

            var defaultGageHeightUnit = Substitute.For<IUnit>();
            GageHeightParameter.DefaultUnit.Returns(defaultGageHeightUnit);

            var defaultGageHeightMethod = Substitute.For<IMonitoringMethod>();
            defaultGageHeightMethod.MethodCode.Returns(ParametersAndMethodsConstants.DefaultMonitoringMethod);
            GageHeightParameter.MonitoringMethods.Returns(new List<IMonitoringMethod> { defaultGageHeightMethod });

            _parseContext.GageHeightParameter.Returns(GageHeightParameter);
        }

        private void SetUpOtherParameters()
        {
            var parameters = new List<IParameter>
            {
                CreateMockParameter(ParametersAndMethodsConstants.AreaParameterId),
                CreateMockParameter(ParametersAndMethodsConstants.VelocityParameterId),
                CreateMockParameter(ParametersAndMethodsConstants.DistanceToGageParameterId),
                CreateMockParameter(ParametersAndMethodsConstants.WidthParameterId)
            };

            _parseContext.AllParameters.ReturnsForAnyArgs(parameters);
        }

        private static IParameter CreateMockParameter(string parameterId)
        {
            var parameter = Substitute.For<IParameter>();
            parameter.Id.Returns(parameterId);

            var defaultUnit = Substitute.For<IUnit>();
            parameter.DefaultUnit.Returns(defaultUnit);

            var defaultMethod = Substitute.For<IMonitoringMethod>();
            defaultMethod.MethodCode.Returns(ParametersAndMethodsConstants.DefaultMonitoringMethod);
            parameter.MonitoringMethods.Returns(new List<IMonitoringMethod> { defaultMethod });

            return parameter;
        }
    }
}
