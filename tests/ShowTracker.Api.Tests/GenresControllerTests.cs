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
	public class GenresControllerTests
	{
		private readonly IFixture _fixture;
		private readonly Mock<IGenreService> _genreServiceMock;
		private readonly GenresController _controller;

		public GenresControllerTests()
		{
			_fixture = new Fixture().Customize(new AutoMoqCustomization());
			_genreServiceMock = _fixture.Freeze<Mock<IGenreService>>();
			_controller = new GenresController(_genreServiceMock.Object);
		}

		[Fact]
		public async Task GetAllGenres_ReturnsOkWithGenres()
		{
			// Arrange
			var genres = _fixture.CreateMany<GenreDto>(5).ToList();
			_genreServiceMock.Setup(s => s.GetAllGenresAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(genres);

			// Act
			var result = await _controller.GetAllGenres(null, false, 1, 10);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnValue = Assert.IsAssignableFrom<List<GenreDto>>(okResult.Value!);
			Assert.Equal(5, returnValue.Count);
		}

		[Fact]
		public async Task GetGenre_WhenFound_ReturnsOkWithGenre()
		{
			// Arrange
			var genre = _fixture.Create<GenreDto>();
			_genreServiceMock.Setup(s => s.GetGenreByIdAsync(genre.Id)).ReturnsAsync(genre);

			// Act
			var result = await _controller.GetGenre(genre.Id);

			// Assert
			var okResult = Assert.IsType<OkObjectResult>(result.Result);
			var returnValue = Assert.IsType<GenreDto>(okResult.Value!);
			Assert.Equal(genre.Id, returnValue.Id);
		}

		[Fact]
		public async Task GetGenre_WhenNotFound_ReturnsNotFound()
		{
			// Arrange
			_genreServiceMock.Setup(s => s.GetGenreByIdAsync(It.IsAny<int>())).ReturnsAsync((GenreDto?)null);

			// Act
			var result = await _controller.GetGenre(1);

			// Assert
			Assert.IsType<NotFoundResult>(result.Result);
		}

		[Fact]
		public async Task CreateGenre_ValidDto_ReturnsCreatedAtAction()
		{
			// Arrange
			var dto = _fixture.Create<CreateGenreDto>();
			var created = _fixture.Create<GenreDto>();
			_genreServiceMock.Setup(s => s.CreateGenreAsync(dto)).ReturnsAsync(created);

			// Act
			var result = await _controller.CreateGenre(dto);

			// Assert
			var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
			Assert.Equal(nameof(GenresController.GetGenre), createdAtActionResult.ActionName);
			Assert.Equal(created.Id, createdAtActionResult.RouteValues["id"]);
		}

		[Fact]
		public async Task CreateGenre_InvalidDto_ReturnsBadRequest()
		{
			// Arrange
			var dto = new CreateGenreDto { Name = "" };

			// Act
			var result = await _controller.CreateGenre(dto);

			// Assert
			Assert.IsType<BadRequestObjectResult>(result.Result);
		}
	}
}