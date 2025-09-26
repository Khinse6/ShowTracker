using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ShowTracker.Api.Controllers;
using ShowTracker.Api.Dtos;
using ShowTracker.Api.Services;
using Xunit;

namespace ShowTracker.Api.Tests
{
	public class ShowsControllerTests
	{
		private readonly Mock<IShowService> _mockService;
		private readonly ShowsController _controller;

		public ShowsControllerTests()
		{
			_mockService = new Mock<IShowService>();
			_controller = new ShowsController(_mockService.Object);
		}

		[Fact]
		public async Task GetAllShows_ReturnsOkWithShows()
		{
			// Arrange
			var shows = new List<ShowSummaryDto>
						{
								new() { Id = 1, Title = "Show1", Description = "Desc1", ReleaseDate = new DateOnly(2023, 1, 1) },
								new() { Id = 2, Title = "Show2", Description = "Desc2", ReleaseDate = new DateOnly(2023, 1, 2) }
						};
			_mockService.Setup(s => s.GetAllShowsAsync()).ReturnsAsync(shows);

			// Act
			var result = await _controller.GetAllShows();

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnShows = Assert.IsType<List<ShowSummaryDto>>(okResult.Value);
			Assert.Equal(2, returnShows.Count);
		}

		[Fact]
		public async Task GetShow_ReturnsOk_WhenShowExists()
		{
			// Arrange
			var show = new ShowDetailsDto
			{
				Id = 1,
				Title = "Show1",
				Description = "Desc1",
				ReleaseDate = new DateOnly(2023, 1, 1)
			};
			_mockService.Setup(s => s.GetShowByIdAsync(1)).ReturnsAsync(show);

			// Act
			var result = await _controller.GetShow(1);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnShow = Assert.IsType<ShowDetailsDto>(okResult.Value);
			Assert.Equal(1, returnShow.Id);
		}

		[Fact]
		public async Task GetShow_ReturnsNotFound_WhenShowDoesNotExist()
		{
			// Arrange
			_mockService.Setup(s => s.GetShowByIdAsync(1)).ReturnsAsync((ShowDetailsDto?)null);

			// Act
			var result = await _controller.GetShow(1);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task CreateShow_ReturnsCreatedAtRoute()
		{
			// Arrange
			var createDto = new ShowCreateDto
			{
				Title = "New Show",
				Description = "Description",
				ReleaseDate = new DateOnly(2023, 1, 1)
			};

			var createdDto = new ShowSummaryDto
			{
				Id = 1,
				Title = "New Show",
				Description = "Description",
				ReleaseDate = new DateOnly(2023, 1, 1)
			};
			_mockService.Setup(s => s.CreateShowAsync(createDto)).ReturnsAsync(createdDto);

			// Act
			var result = await _controller.CreateShow(createDto);

			// Assert
			var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
			Assert.Equal("GetShow", createdResult.RouteName);
			var returnDto = Assert.IsType<ShowSummaryDto>(createdResult.Value);
			Assert.Equal(1, returnDto.Id);
		}

		[Fact]
		public async Task UpdateShow_ReturnsNoContent_WhenShowExists()
		{
			// Arrange
			var updateDto = new ShowUpdateDto
			{
				Title = "Updated Show",
				Description = "Updated",
				ReleaseDate = new DateOnly(2023, 1, 1)
			};

			_mockService.Setup(s => s.UpdateShowAsync(1, updateDto)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.UpdateShow(1, updateDto);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task UpdateShow_ReturnsNotFound_WhenShowDoesNotExist()
		{
			// Arrange
			var updateDto = new ShowUpdateDto
			{
				Title = "Updated Show",
				Description = "Updated",
				ReleaseDate = new DateOnly(2023, 1, 1)
			};

			_mockService.Setup(s => s.UpdateShowAsync(1, updateDto))
									.ThrowsAsync(new KeyNotFoundException());

			// Act
			var result = await _controller.UpdateShow(1, updateDto);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}

		[Fact]
		public async Task DeleteShow_ReturnsNoContent_WhenShowExists()
		{
			// Arrange
			_mockService.Setup(s => s.DeleteShowAsync(1)).Returns(Task.CompletedTask);

			// Act
			var result = await _controller.DeleteShow(1);

			// Assert
			Assert.IsType<NoContentResult>(result);
		}

		[Fact]
		public async Task DeleteShow_ReturnsNotFound_WhenShowDoesNotExist()
		{
			// Arrange
			_mockService.Setup(s => s.DeleteShowAsync(1))
									.ThrowsAsync(new KeyNotFoundException());

			// Act
			var result = await _controller.DeleteShow(1);

			// Assert
			Assert.IsType<NotFoundResult>(result);
		}
	}
}
