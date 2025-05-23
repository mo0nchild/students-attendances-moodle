import type { CourseInfoModel } from "@core/models/course-models"
import type { LessonInfoModel } from "@core/models/lesson-models"
import { lessonService } from "@services/LessonService"
import { AxiosError } from "axios"
import type { StateCreator } from "zustand"

export interface LessonState {
  lessons: LessonInfoModel[]
  course: CourseInfoModel | null
  isLoading: boolean
  error?: string
}

export interface LessonAction {
  fetchLessons: (courseId: number) => Promise<void>
  setCourse: (course: CourseInfoModel) => void
}

const initialLessonsState: LessonState = {
  lessons: [],
  isLoading: false,
  course: null,
  error: undefined
}
const refreshLessonsState: Partial<LessonState> = {
  lessons: [],
  isLoading: false,
  error: undefined
}

export type LessonSlice = LessonState & LessonAction

export const createLessonSlice: StateCreator<LessonSlice, [], [], LessonSlice> = (set) => ({
  setCourse: (course: CourseInfoModel) => set({course: course}),
  fetchLessons: async (courseId: number) => {
    try {
			set({ error: undefined, isLoading: true })
			const lessonsList = (await lessonService.getLessonsList(courseId)).data

			set({ isLoading: false, lessons: lessonsList })
    }
    catch (error) {
      if (error instanceof AxiosError) {
        set({ 
          ...refreshLessonsState, 
          error: error.response ? 'Невозможно получить уроки' : error.message
        })
      } else {
        set({ ...refreshLessonsState, error: 'Неизвестная ошибка' })
      }
      console.log(error)
    }
  },
  ...initialLessonsState
})