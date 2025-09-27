using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShowTracker.Api.Controllers;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ShowTracker.Api.Tests.Controllers
{
	public class ShowsControllerTests
	{
		private readonly IFixture _fixture;
		private readonly Mock<IShowService> _showServiceMock;
		private readonly ShowsController _controller;

		public ShowsControllerTests()
		{
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
			_showServiceMock = _fixture.Freeze<Mock<IShowService>>();
			_controller = new ShowsController(_showServiceMock.Object);

			// Customize DateOnly globally to generate valid random dates
			_fixture.Customize<DateOnly>(c => c.FromFactory(() =>
			{
				var today = DateTime.Today;
				var random = new Random();
				int year = random.Next(2000, today.Year + 1);
				int month = random.Next(1, 13);
				int day = random.Next(1, DateTime.DaysInMonth(year, month) + 1);
				return new DateOnly(year, month, day);
			}));
		}

		[Fact]
		public async Task GetAllShows_ReturnsOkWithShows()
		{
			var shows = _fixture.CreateMany<ShowSummaryDto>(3).ToList();
			_showServiceMock.Setup(s => s.GetAllShowsAsync()).ReturnsAsync(shows);

			var result = await _controller.GetAllShows();

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnValue = Assert.IsAssignableFrom<List<ShowSummaryDto>>(okResult.Value!);
			Assert.Equal(3, returnValue.Count);
		}

		[Fact]
		public async Task GetShow_WhenFound_ReturnsOkWithShow()
		{
			var show = _fixture.Create<ShowDetailsDto>();
			_showServiceMock.Setup(s => s.GetShowByIdAsync(show.Id)).ReturnsAsync(show);

			var result = await _controller.GetShow(show.Id);

			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnValue = Assert.IsType<ShowDetailsDto>(okResult.Value!);
			Assert.Equal(show.Id, returnValue.Id);
		}

		[Fact]
		public async Task GetShow_WhenNotFound_ReturnsNotFound()
		{
			_showServiceMock.Setup(s => s.GetShowByIdAsync(It.IsAny<int>())).ReturnsAsync((ShowDetailsDto?)null);

			var result = await _controller.GetShow(1);

			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task CreateShow_ValidDto_ReturnsCreatedAtRoute()
		{
			var dto = _fixture.Create<CreateShowDto>();
			var created = _fixture.Build<ShowSummaryDto>()
														.With(x => x.Id, 1)
														.With(x => x.ReleaseDate, dto.ReleaseDate)
														.Create();

			_showServiceMock.Setup(s => s.CreateShowAsync(dto)).ReturnsAsync(created);

			var result = await _controller.CreateShow(dto);

			var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal("GetShow", createdAtRouteResult.RouteName);
			Assert.Equal(created.Id, createdAtRouteResult.RouteValues["id"]);
		}

		[Theory]
		[InlineData("", "Some description")]
		[InlineData("Title", "")]
		[InlineData("", "")]
		public async Task CreateShow_InvalidDto_ReturnsBadRequest(string title, string description)
		{
			var dto = new CreateShowDto
			{
				Title = title,
				Description = description,
				ReleaseDate = DateOnly.FromDateTime(DateTime.Today)
			};

			var result = await _controller.CreateShow(dto);

			Assert.IsType<BadRequestObjectResult>(result.Result);
		}

		[Fact]
		public async Task UpdateShow_ValidDto_ReturnsNoContent()
		{
			var dto = _fixture.Create<UpdateShowDto>();
			_showServiceMock.Setup(s => s.UpdateShowAsync(1, It.IsAny<UpdateShowDto>())).Returns(Task.CompletedTask);

			var result = await _controller.UpdateShow(1, dto);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateShow_NotFound_ReturnsNotFound()
		{
			var dto = _fixture.Create<UpdateShowDto>();
			_showServiceMock.Setup(s => s.UpdateShowAsync(1, It.IsAny<UpdateShowDto>())).Throws<KeyNotFoundException>();

			var result = await _controller.UpdateShow(1, dto);

			Assert.IsType<NotFoundResult>(result);
		}

		[Theory]
		[InlineData("", "Some description")]
		[InlineData("Title", "")]
		[InlineData("", "")]
		public async Task UpdateShow_InvalidDto_ReturnsBadRequest(string title, string description)
		{
			var dto = new UpdateShowDto
			{
				Title = title,
				Description = description,
				ReleaseDate = DateOnly.FromDateTime(DateTime.Today)
			};

			var result = await _controller.UpdateShow(1, dto);

			Assert.IsType<BadRequestObjectResult>(result);
		}

		[Fact]
		public async Task DeleteShow_WhenExists_ReturnsNoContent()
		{
			_showServiceMock.Setup(s => s.DeleteShowAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

			var result = await _controller.DeleteShow(1);

			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteShow_WhenNotFound_ReturnsNotFound()
		{
			_showServiceMock.Setup(s => s.DeleteShowAsync(It.IsAny<int>())).Throws<KeyNotFoundException>();

			var result = await _controller.DeleteShow(1);

			Assert.IsType<NotFoundResult>(result);
		}
	}
}
